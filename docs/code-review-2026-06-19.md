# Code Review — 2026-06-19

Commits couverts : `HEAD~4...HEAD` (CRUD endpoints, tests, suppression annonces/AO, login admin)

---

## Findings (sévérité décroissante)

### 🔴 1. IDOR — candidatures d'autres entreprises accessibles

**Fichier :** `src/Modules/CatalogueEmploi/Application/Features/ObtenirCandidatures/ObtenirCandidaturesQueryHandler.cs` L20

Handler vérifie que l'annonce existe mais ne vérifie jamais `annonce.EntrepriseId == request.UtilisateurId`. Toute entreprise avec la permission `ConsulterCandidaturesRecues` peut lire les candidatures de n'importe quelle autre entreprise en fournissant un `AnnonceId` arbitraire.

**Fix :** après le fetch de l'annonce, ajouter `if (annonce.EntrepriseId != request.UtilisateurId) throw new PermissionRefuseeException(...)`.

---

### 🔴 2. Login admin bloqué si même email existe en candidat

**Fichier :** `src/Modules/GestionIdentite/Application/Features/ConnecterUtilisateur/ConnecterUtilisateurQueryHandler.cs` L21

Le bloc candidat est évalué en premier. Si un email admin existe aussi dans `ProfilsCandidats` (collision ou attaque), le handler retourne un token candidat et n'atteint jamais le bloc admin (L34-39). L'admin est définitivement bloqué.

**Fix :** tester le login admin en priorité, ou séparer les endpoints candidat/admin.

---

### 🔴 3. Admin désactivé reçoit un JWT valide

**Fichier :** `ConnecterUtilisateurQueryHandler.cs` L34

`ObtenirAdministrateurParEmailAsync` ne filtre pas sur un statut actif. Un compte admin révoqué avec mot de passe inchangé obtient un token pleinement valide.

**Fix :** ajouter un champ `EstActif` sur `Administrateur` et filtrer dans la requête, ou vérifier après fetch.

---

### 🟠 4. `UnauthorizedAccessException` → HTTP 500 sur tout échec de login

**Fichier :** `src/CVTech.Api/Program.cs` L138 + `ConnecterUtilisateurQueryHandler.cs` L41

Le switch d'erreurs ne couvre pas `UnauthorizedAccessException` (toujours utilisée sur l'échec de login). Elle tombe dans `_ => (500, ...)`. Chaque login raté retourne HTTP 500 au client.

**Fix :** ajouter `UnauthorizedAccessException e => (401, e.Message)` dans le switch, ou remplacer le throw par une exception domaine dédiée.

---

### 🟠 5. Migration vide appliquée — schema `Administrateurs` probablement absent

**Fichier :** `src/Modules/GestionIdentite/Infrastructure/Persistence/Migrations/20260616103101_PendingChanges.cs` L11

`Up()` et `Down()` sont vides. EF va marquer la migration comme appliquée sans créer la table `Administrateurs`. Le nouveau code de login admin plantera en production avec table-not-found.

**Fix :** supprimer cette migration (`dotnet ef migrations remove`), vérifier le model, régénérer proprement.

---

### 🟡 6. `!.Value` sur claim absent → NullReferenceException → 500

**Fichier :** `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs` L39 (+ L55, L63, L79, L87 ; `AppelOffreController` L39, L47, L55)

```csharp
var utilisateurId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
```

Si le JWT passe `[Authorize]` mais ne contient pas `NameIdentifier`, `FindFirst` retourne null, le `!` supprime l'avertissement compilateur, `.Value` lève `NullReferenceException` → HTTP 500.

**Fix :** utiliser `FindFirst(...)?.Value ?? throw new InvalidOperationException(...)`, ou une méthode d'extension partagée sur un `ApiControllerBase`.

---

### 🟡 7. `PermissionRefuseeException.Message` expose le GUID interne dans les 403

**Fichier :** `src/CVTech.Api/Program.cs` L148

Le switch passe `e.Message` directement dans la réponse JSON. `PermissionRefuseeException` inclut l'ID utilisateur dans son message (`L'utilisateur {utilisateurId} n'est pas autorisé...`). Chaque 403 expose une clé primaire interne.

**Fix :** utiliser un message générique (`"Permission refusée."`) dans la réponse, logger le détail serveur-side.

---

### 🟡 8. Suppression annonce/AO réservée aux admins — entreprises ne peuvent pas supprimer leur propre contenu

**Fichiers :** `SupprimerAnnonceCommandHandler.cs` L14 / `SupprimerAppelOffreCommandHandler.cs` L14

Les deux handlers vérifient `ModererContenu` (admin seulement). Une entreprise ne peut pas supprimer sa propre annonce ou son propre appel d'offre sans aide admin. De plus, aucun check d'existence n'est fait avant suppression : un ID inexistant retourne 204 silencieusement.

**Fix (si l'intention est owner-or-admin) :** vérifier `PublierAnnonceEmploi` + ownership, ou `ModererContenu` pour admin. Ajouter un throw `AnnonceIntrouvableException` si entité not found.

---

### 🟡 9. Permission d'écriture utilisée pour une lecture (`mes-annonces`)

**Fichier :** `src/Modules/CatalogueEmploi/Application/Features/ObtenirAnnoncesEntreprise/ObtenirAnnoncesEntrepriseQueryHandler.cs` L15

`PublierAnnonceEmploi` gate un GET. Révoquer la permission de publication bloque aussi la lecture des annonces existantes.

**Fix :** introduire une permission `ConsulterSesAnnonces`, ou au minimum utiliser la permission avec sémantique la plus large déjà disponible pour ce rôle.

---

### 🟢 10. `ListerParEntrepriseAsync` non paginée — risque OOM/timeout

**Fichier :** `src/Modules/CatalogueEmploi/Infrastructure/Repositories/AnnonceRepository.cs` L21

```csharp
=> await db.Annonces.Where(a => a.EntrepriseId == entrepriseId).ToListAsync(ct);
```

Retourne la liste complète sans pagination ni projection. Une grande entreprise peut saturer mémoire ou temps DB.

**Fix :** ajouter `Take(n)` / pagination, ou une projection vers un DTO léger.

---

## Résumé

| # | Sévérité | Catégorie | Titre court |
|---|----------|-----------|-------------|
| 1 | 🔴 Critique | Sécurité | IDOR candidatures |
| 2 | 🔴 Critique | Auth | Login admin bloqué par collision email |
| 3 | 🔴 Critique | Auth | Admin désactivé → JWT valide |
| 4 | 🟠 Élevé | HTTP | Login raté → HTTP 500 |
| 5 | 🟠 Élevé | DB | Migration vide, schema absent |
| 6 | 🟡 Moyen | Robustesse | Null deref sur claim → 500 |
| 7 | 🟡 Moyen | Sécurité | GUID utilisateur exposé en 403 |
| 8 | 🟡 Moyen | Logique métier | Suppression admin-only, 204 fantôme |
| 9 | 🟡 Moyen | Permissions | Permission write pour read |
| 10 | 🟢 Faible | Perf | Liste non paginée |

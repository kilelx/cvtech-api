# Console Admin — Design Spec

## Scope

Build a functional admin console for CVTech with:
- Publication d'articles éditoriaux
- Modération des annonces d'emploi (suppression)
- Modération des appels d'offre freelance (suppression)

## Backend

### 1. SupprimerAnnonce (CatalogueEmploi)

**Feature:** `src/Modules/CatalogueEmploi/Application/Features/SupprimerAnnonce/`
- `SupprimerAnnonceCommand(utilisateurId: Guid, annonceId: Guid)`
- Handler: vérifie `IVerificateurPermission` (rôle Administrateur requis), supprime via `IAnnonceRepository`

**Repository:** Ajouter `SupprimerAsync(Guid id, CancellationToken)` à `IAnnonceRepository` + implémentation EF Core dans `AnnonceRepository`

**Controller:** `DELETE /api/annonces/{id}` dans `AnnonceController`

### 2. SupprimerAppelOffre (AppelOffreFreelance)

**Feature:** `src/Modules/AppelOffreFreelance/Application/Features/SupprimerAppelOffre/`
- `SupprimerAppelOffreCommand(utilisateurId: Guid, appelOffreId: Guid)`
- Handler: même pattern permission, supprime via `IAppelOffreRepository`

**Repository:** Ajouter `SupprimerAsync(Guid id, CancellationToken)` à `IAppelOffreRepository` + implémentation

**Controller:** `DELETE /api/appels-offre/{id}` dans `AppelOffreController`

### 3. ListerArticles (ActualiteEtAbonnement)

**Feature:** `src/Modules/ActualiteEtAbonnement/Application/Features/ListerArticles/`
- `ListerArticlesQuery()` → retourne liste d'articles

**Controller:** `GET /api/actualites` dans `ActualiteController` (anonyme ou admin, pour affichage)

## Frontend

### Sidebar admin (`app/admin/layout.tsx`)

Ajouter liens :
- `/admin/actualites/publier` — Publier un article
- `/admin/annonces` — Modération annonces
- `/admin/appels-offre` — Modération appels d'offre

### Pages

**`app/admin/actualites/publier/page.tsx`**
- Formulaire : Titre, Contenu (textarea), Domaine métier (input)
- Submit → `POST /api/actualites` avec token JWT
- Redirect ou message succès

**`app/admin/annonces/page.tsx`**
- Fetch `GET /api/annonces` → liste toutes les annonces
- Colonnes : Titre, Entreprise, Type contrat, Date, Domaine
- Bouton "Supprimer" par ligne → `DELETE /api/annonces/{id}` → refresh liste

**`app/admin/appels-offre/page.tsx`**
- Fetch `GET /api/appels-offre` → liste tous les AO
- Colonnes : Titre, Entreprise, Budget, Deadline, Domaine
- Bouton "Supprimer" par ligne → `DELETE /api/appels-offre/{id}` → refresh liste

## Permissions

- `SupprimerAnnonceCommand` : rôle `Administrateur` uniquement
- `SupprimerAppelOffreCommand` : rôle `Administrateur` uniquement
- `PublierArticleCommand` : déjà implémenté, rôle `Administrateur`
- Frontend : pages `/admin/*` déjà protégées par middleware existant

## Architecture constraints

- Handlers doivent appeler `IVerificateurPermission` en première ligne (règle projet)
- Lever `PermissionRefuseeException` si refusé (pas `UnauthorizedAccessException`)
- Nommage domaine en français, technique en anglais

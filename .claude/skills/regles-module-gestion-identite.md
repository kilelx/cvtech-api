---
name: cvtech-gestion-identite-guard
description: Impose les règles d'implémentation du module GestionIdentite — seul détenteur de la matrice des rôles, expose IVerificateurPermission aux autres modules via contrat public
globs: "src/Modules/GestionIdentite/**/*.cs"
---
# CONTEXTE
`GestionIdentite` est le module central de sécurité. Il est le SEUL à connaître et implémenter la matrice des rôles et permissions. Les autres modules ne lisent jamais sa base de données directement : ils consomment uniquement le contrat public `IVerificateurPermission` injecté via le `ModuleLoader`.

# RÈGLES STRICTES DU MODULE GESTION IDENTITE

## 1. CONTRAT PUBLIC — IVerificateurPermission
*   L'interface `IVerificateurPermission` est définie dans `Contracts/` et est le seul point d'entrée consommable par les autres modules.
*   Signature minimale obligatoire :

```csharp
// Contracts/IVerificateurPermission.cs
public interface IVerificateurPermission
{
    Task<bool> AutoriserAsync(Guid utilisateurId, ActionSecurisee action, CancellationToken cancellationToken = default);
}
```

*   `ActionSecurisee` est une enum exhaustive couvrant toutes les actions de la matrice de permissions.

## 2. MATRICE DES PERMISSIONS — IMPLÉMENTATION OBLIGATOIRE
L'implémentation de `IVerificateurPermission` doit refléter exactement cette matrice :

| Action (`ActionSecurisee`) | Visiteur | Candidat | Entreprise | Administrateur |
|---|---|---|---|---|
| `ConsulterAnnonce` | ✅ | ✅ | ✅ | ✅ |
| `ConsulterFluxRSS` | ✅ | ✅ | ✅ | ✅ |
| `ModifierCV` | ❌ | ✅ | ❌ | ✅ |
| `PostulerAnnonce` | ❌ | ✅ | ❌ | ❌ |
| `SoumettrePropositionFreelance` | ❌ | ✅ | ❌ | ❌ |
| `PublierAnnonceEmploi` | ❌ | ❌ | ✅ (siennes) | ✅ |
| `PublierAppelOffre` | ❌ | ❌ | ✅ (siens) | ✅ |
| `ConsulterCandidaturesRecues` | ❌ | ❌ | ✅ (siennes) | ✅ |
| `SAbonnerDomaineMetier` | ❌ | ✅ | ✅ | ✅ |
| `PublierArticleActualite` | ❌ | ❌ | ❌ | ✅ |
| `ModererContenu` | ❌ | ❌ | ❌ | ✅ |
| `BloquerCompte` | ❌ | ❌ | ❌ | ✅ |
| `GererDomainesMetier` | ❌ | ❌ | ❌ | ✅ |

## 3. ENTITÉS DU DOMAINE
*   `ProfilCandidat` : données personnelles, CV lié, statut du compte (actif/bloqué).
*   `ProfilEntreprise` : raison sociale, SIRET, statut.
*   `Administrateur` : hérite de tous les droits, pas de profil métier supplémentaire.
*   `RoleUtilisateur` : enum `Candidat | Entreprise | Administrateur`.
*   `MatricePermissions` : Value Object ou service domaine portant la logique d'autorisation.
*   `PermissionRefuseeException` : exception métier levée quand un Handler refuse une action.

## 4. ISOLATION — LES AUTRES MODULES N'ACCÈDENT PAS À LA BASE
*   `GestionIdentite` ne partage JAMAIS son `DbContext` ou ses entités avec un autre module.
*   Si un autre module a besoin de vérifier un rôle ou une identité, il passe par `IVerificateurPermission` — jamais par une requête SQL directe.

## 5. ENREGISTREMENT VIA MODULELOADER
*   Le `ModuleLoader.cs` de `GestionIdentite` enregistre l'implémentation concrète et expose l'interface :

```csharp
// Loader/ModuleLoader.cs
services.AddScoped<IVerificateurPermission, VerificateurPermission>();
// Les autres ModuleLoader injectent IVerificateurPermission sans connaître VerificateurPermission
```

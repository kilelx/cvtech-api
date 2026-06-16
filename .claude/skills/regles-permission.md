---
name: cvtech-permission-guard
description: Vérifie que chaque cas d'usage contrôle les permissions avant l'action métier
globs: "src/Modules/**/Application/Features/**/*.cs"
---
# CONTEXTE
Le système gère trois rôles : Candidat, Entreprise, Administrateur. La sécurité est centralisée dans le module `GestionIdentite` qui expose le contrat public `IVerificateurPermission`.

# INSTRUCTIONS ET RÈGLES DE SÉCURITÉ STRICTES

En tant qu'assistant IA, tu DOIS appliquer ce protocole de sécurité pour CHAQUE Handler (Command ou Query) généré ou modifié :

## 1. INJECTION OBLIGATOIRE
Chaque constructeur de Handler métier doit injecter l'interface `IVerificateurPermission` (et l'utilisateur courant ou son ID si nécessaire au contexte).

## 2. VÉRIFICATION EN LIGNE 1
La TOUTE PREMIÈRE instruction de la méthode `Handle` doit être un appel à `IVerificateurPermission`. Aucune logique métier, aucune requête en base de données ne doit s'exécuter avant cette vérification.

## 3. REJET ET EXCEPTION MÉTIER
Si la vérification échoue, le Handler doit immédiatement lever une exception métier en français : `PermissionRefuseeException`. Ne retourne jamais de codes HTTP directement depuis la couche Application.

## 4. MATRICE DES PERMISSIONS À RESPECTER
Tu dois utiliser ces règles pour déduire si une action est autorisée :
*   **Visiteur Anonyme** : Peut uniquement consulter les annonces, les appels d'offre et le fil RSS d'actualité.
*   **Candidat** : Peut constituer son CV, postuler aux annonces, soumettre une proposition freelance, et s'abonner aux domaines métiers.
*   **Entreprise** : Peut publier SES annonces/appels d'offre, consulter LES SIENNES (candidatures reçues), et s'abonner aux domaines.
*   **Administrateur** : A tous les droits (modération, blocage de compte, publication d'articles d'actualité, gestion des domaines).

## EXEMPLE DE STRUCTURE ATTENDUE
```csharp
public async Task<Result> Handle(PostulerAnnonceCommand request, CancellationToken cancellationToken)
{
    if (!await _verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PostulerAnnonce))
    {
        throw new PermissionRefuseeException("Seul un candidat actif peut postuler à une annonce.");
    }
    // ... suite de la logique métier
}
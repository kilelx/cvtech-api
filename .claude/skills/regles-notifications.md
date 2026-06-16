---
name: cvtech-notifications-guard
description: Impose les règles du système d'abonnement et de notification par domaine métier — déclenchement sur événements, isolation des canaux, ciblage précis des abonnés
globs: "src/Modules/ActualiteEtAbonnement/**/*.cs"
---
# CONTEXTE
Le sous-domaine D.2 du module `ActualiteEtAbonnement` gère les abonnements et notifications. Un utilisateur authentifié (Candidat ou Entreprise) s'abonne à un `DomaineMetier`. Quand une annonce ou un appel d'offre est publié dans ce domaine, tous les abonnés concernés reçoivent une notification.

# RÈGLES STRICTES DU SYSTÈME DE NOTIFICATION

## 1. DÉCLENCHEMENT UNIQUEMENT PAR ÉVÉNEMENTS
*   Les notifications ne sont JAMAIS déclenchées par un appel HTTP direct.
*   Elles s'activent exclusivement via les handlers d'événements MediatR :
    *   `INotificationHandler<AnnoncePubliee>` → notifie les abonnés du domaine de l'annonce
    *   `INotificationHandler<AppelOffrePublie>` → notifie les abonnés du domaine de l'appel d'offre
*   Cf. skill `cvtech-bus-evenements` pour la structure des handlers.

## 2. CIBLAGE DES ABONNÉS
*   Seuls les abonnés dont le `DomaineMetier` correspond au domaine de l'événement reçoivent une notification.
*   Un abonné ne reçoit jamais de notification pour un domaine auquel il n'est pas inscrit.
*   La liste des abonnés est chargée depuis le repository `IAbonnementRepository` — jamais depuis un autre module.

```csharp
public async Task Handle(AnnoncePubliee notification, CancellationToken cancellationToken)
{
    var abonnes = await _abonnementRepository.ObtenirAbonnesParDomaine(notification.DomaineMetier, cancellationToken);
    foreach (var abonne in abonnes)
    {
        await _serviceNotification.EnvoyerAsync(abonne, notification, cancellationToken);
    }
}
```

## 3. ENTITÉS DU DOMAINE
*   `Abonnement` : lie un `UtilisateurId` à un `DomaineMetier`. Un utilisateur peut avoir plusieurs abonnements.
*   `Notification` : représente un message envoyé à un abonné (titre, corps, date, canal, statut lu/non-lu).
*   `CanalDiffusion` : enum `Email | InApp` — l'utilisateur choisit son canal dans ses `PreferenceNotification`.
*   `DomaineMetier` : Value Object partagé avec les modules B et C via contrat public. Ne pas redéfinir localement.

## 4. CANAUX DE DIFFUSION
*   Le module supporte au minimum un canal : `Email` (SMTP de test / console) **ou** `InApp` (WebSocket ou polling).
*   `IServiceNotification` est l'abstraction injectée. L'implémentation concrète est dans `Infrastructure/`.
*   Ne jamais coupler le Handler à une implémentation SMTP ou WebSocket directement.

## 5. GESTION DES ABONNEMENTS (CRUD)
*   Un utilisateur authentifié peut s'abonner (`POST /abonnements`) et se désabonner (`DELETE /abonnements/{domaineMetier}`).
*   Vérification de permission obligatoire avant toute action (cf. skill `cvtech-permission-guard`) : Visiteur anonyme = accès refusé.
*   Un abonnement en doublon (même utilisateur, même domaine) doit être ignoré silencieusement ou retourner `409 Conflict`.

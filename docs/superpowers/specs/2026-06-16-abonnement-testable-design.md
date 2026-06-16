# Design : Système d'abonnement testable end-to-end

**Date :** 2026-06-16  
**Scope :** Module `ActualiteEtAbonnement` — complétion pour test end-to-end  
**Approche choisie :** A — Backend complet + frontend minimal

## Contexte

Le flux de souscription (`POST /api/abonnements`) et les handlers d'événements (`SurAnnoncePublieeHandler`, `SurAppelOffrePublieHandler`) existent. Manque : persistance des notifications, endpoints de lecture/suppression, UI de liste.

## Architecture

### Domaine

**`IAbonnementRepository`** — ajout de 2 méthodes :
- `ObtenirParUtilisateurAsync(Guid userId)` → `IReadOnlyList<Abonnement>`
- `SupprimerAsync(Guid id, Guid userId)` → void (vérifie ownership, lève exception si non trouvé)

**`INotificationRepository`** — nouvelle interface :
- `AjouterAsync(Notification notif)`
- `ObtenirParUtilisateurAsync(Guid userId)` → `IReadOnlyList<Notification>`
- `MarquerCommeLueAsync(Guid id, Guid userId)`

### Application

| Classe | Type | Description |
|--------|------|-------------|
| `ObtenirAbonnementsQuery` | Query MediatR | Renvoie abonnements d'un user |
| `ObtenirAbonnementsQueryHandler` | Handler | Appelle `IAbonnementRepository.ObtenirParUtilisateurAsync` |
| `SupprimerAbonnementCommand` | Command MediatR | Désabonner — vérifie ownership |
| `SupprimerAbonnementCommandHandler` | Handler | Appelle `IAbonnementRepository.SupprimerAsync` |
| `ObtenirNotificationsQuery` | Query MediatR | Renvoie notifs InApp d'un user |
| `ObtenirNotificationsQueryHandler` | Handler | Appelle `INotificationRepository.ObtenirParUtilisateurAsync` |
| `MarquerNotificationLueCommand` | Command MediatR | Marque une notif comme lue |
| `MarquerNotificationLueCommandHandler` | Handler | Appelle `INotificationRepository.MarquerCommeLueAsync` |

### Infrastructure

**`AbonnementRepository`** — ajout des 2 nouvelles méthodes.

**`NotificationRepository`** — nouvelle classe EF Core sur `ActualiteDbContext.Notifications`.

**`ServiceNotificationDb`** — remplace `ServiceNotificationConsole` :
- Persiste toujours un `Notification` en DB via `INotificationRepository`
- Si `canal == Email` : log console en plus (simulation envoi email)
- Enregistré comme `IServiceNotification` dans `ModuleLoader`

### API

```
GET    /api/abonnements          → liste abonnements user authentifié
DELETE /api/abonnements/{id}     → désabonner (ownership vérifié)
GET    /api/notifications        → liste notifs InApp user authentifié
PATCH  /api/notifications/{id}/lue → marquer comme lue
```

**`AbonnementController`** — ajout `GET` et `DELETE`.  
**`NotificationsController`** — nouveau controller : `GET` + `PATCH /{id}/lue`.

### Frontend

Page `/candidat/abonnements` mise à jour :
- Au mount : `GET /api/abonnements` → affiche liste (domaine + canal + bouton Supprimer)
- Bouton Supprimer → `DELETE /api/abonnements/{id}` → retire de la liste
- Formulaire d'ajout reste présent (inchangé)

## Flux testable end-to-end

1. Se connecter comme candidat
2. S'abonner à un domaine (`POST /api/abonnements`)
3. Vérifier l'abonnement dans l'UI (GET `/api/abonnements`)
4. Publier une annonce dans ce domaine (via Swagger / API CatalogueEmploi)
5. Vérifier la notification créée (`GET /api/notifications`)
6. Marquer comme lue (`PATCH /api/notifications/{id}/lue`)
7. Se désabonner (`DELETE /api/abonnements/{id}`)

## Contraintes

- Canal Email : log console uniquement (pas de SMTP)
- Pas de nouvelle page frontend (notifications testables via Swagger)
- Ownership strict : un user ne peut supprimer que ses propres abonnements/notifs
- Pas de migration EF supplémentaire : `Notifications` est déjà dans `ActualiteDbContextModelSnapshot`

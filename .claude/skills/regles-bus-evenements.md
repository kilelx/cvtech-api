---
name: cvtech-bus-evenements
description: Impose la communication inter-modules via bus d'événements MediatR (INotification) — interdit toute référence directe entre modules
globs: "src/Modules/**/*.cs"
---
# CONTEXTE
Les 4 modules (GestionIdentite, CatalogueEmploi, AppelOffreFreelance, ActualiteEtAbonnement) sont des boîtes noires étanches. Toute communication entre modules passe EXCLUSIVEMENT par deux mécanismes : le bus d'événements MediatR ou les contrats publics injectés via le ModuleLoader.

# RÈGLES STRICTES DE COMMUNICATION INTER-MODULES

## 1. INTERDICTION DES RÉFÉRENCES DIRECTES
*   **INTERDICTION ABSOLUE** d'utiliser un `using` vers les namespaces `Domain`, `Application`, ou `Infrastructure` d'un autre module.
*   Exemple interdit : `using CVTech.Modules.CatalogueEmploi.Domain.Entities;` depuis le module `ActualiteEtAbonnement`.

## 2. ÉVÉNEMENTS DOMAINE (INotification)
*   Quand un module doit notifier les autres d'un fait métier, il publie un **événement domaine** via MediatR.
*   Les événements sont définis dans un dossier `Contracts/Events/` à la racine du module émetteur et sont les seuls éléments qu'un autre module peut référencer.
*   Nommage en français, au passé composé : `AnnoncePubliee`, `AppelOffrePublie`, `CompteBloque`.

```csharp
// Contracts/Events/AnnoncePubliee.cs (dans le module CatalogueEmploi)
public record AnnoncePubliee(Guid AnnonceId, string DomaineMetier, Guid EntrepriseId) : INotification;
```

## 3. PUBLICATION D'UN ÉVÉNEMENT
*   Le Handler émetteur publie via `IPublisher` (MediatR) après la réussite de l'action métier, jamais avant.

```csharp
// Dans PublierAnnonceCommandHandler.cs (module CatalogueEmploi)
await _publisher.Publish(new AnnoncePubliee(annonce.Id, annonce.DomaineMetier, annonce.EntrepriseId), cancellationToken);
```

## 4. CONSOMMATION D'UN ÉVÉNEMENT
*   Le module consommateur implémente `INotificationHandler<TEvent>` dans sa couche `Application`.
*   Il ne connaît que le contrat de l'événement (`AnnoncePubliee`), jamais les entités internes du module émetteur.

```csharp
// Dans Application/Handlers/SurAnnoncePublieeHandler.cs (module ActualiteEtAbonnement)
public class SurAnnoncePublieeHandler : INotificationHandler<AnnoncePubliee>
{
    public async Task Handle(AnnoncePubliee notification, CancellationToken cancellationToken)
    {
        // Notifier les abonnés du domaine métier concerné
    }
}
```

## 5. CONTRATS PUBLICS (IVerificateurPermission)
*   Les interfaces de service inter-modules (ex: `IVerificateurPermission` du module `GestionIdentite`) sont définies dans `Contracts/` et injectées via le `ModuleLoader`.
*   Le module consommateur ne voit que l'interface, jamais son implémentation concrète.

## CARTOGRAPHIE DES ÉVÉNEMENTS DU PROJET
| Événement          | Émetteur              | Consommateur(s)              |
|--------------------|-----------------------|------------------------------|
| `AnnoncePubliee`   | CatalogueEmploi       | ActualiteEtAbonnement        |
| `AppelOffrePublie` | AppelOffreFreelance   | ActualiteEtAbonnement        |

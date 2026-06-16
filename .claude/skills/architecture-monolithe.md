---
name: cvtech-architecture-guard
description: Force le respect de la structure des couches du projet Plateforme-CVTech
globs: "src/Modules/**/*.cs"
---
# CONTEXTE
Nous sommes en 2026. Le projet utilise .NET 10. Le code doit refléter le langage métier en français pour les couches Domain et Application, tout en gardant les standards industriels (anglais) pour l'infrastructure et la technique (Controllers, Handlers, Infrastructure, ModuleLoader).

# INSTRUCTIONS ET RÈGLES ARCHITECTURALES STRICTES

En tant qu'assistant IA, tu DOIS respecter scrupuleusement les règles suivantes lors de la génération ou de la modification du code :

## 1. SÉPARATION EN 5 COUCHES
Chaque module sous `src/Modules/NomDuModule/` doit respecter cette arborescence stricte :
*   `Client/` : Exposition (Minimal APIs, Controllers, DTOs).
*   `Application/` : Logique des cas d'utilisation (Vertical Slices avec MediatR).
*   `Domain/` : Cœur métier strict (Entités, Objets de Valeur, Interfaces, Exceptions).
*   `Infrastructure/` : Accès aux données (Entity Framework Core, Dapper) et services externes.
*   `Loader/` : Plomberie et Injection de Dépendances (`ModuleLoader.cs`).

## 2. PURETÉ DU DOMAINE (DOMAIN LAYER)
*   **INTERDICTION FORMELLE** d'importer `Microsoft.EntityFrameworkCore`, un `DbContext`, ou toute autre librairie d'accès aux données ou au réseau dans le dossier `Domain/`.
*   Le Domaine ne contient que des classes C# pures (POCO), des interfaces pour l'inversion de dépendance et des exceptions métiers.

## 3. VERTICAL SLICE ARCHITECTURE (APPLICATION LAYER)
*   La couche `Application` est organisée par fonctionnalité (ex: `Features/PostulerAnnonce/`).
*   Chaque dossier de fonctionnalité regroupe sa `Command` (ou `Query`), son `Handler` (implémentant `IRequestHandler` de MediatR), et son `Validator` (FluentValidation).

## 4. ÉTANCHÉITÉ DES MODULES (RÈGLE D'OR)
*   **INTERDICTION ABSOLUE** de faire un `using` ciblant les dossiers `Domain`, `Application`, ou `Infrastructure` d'un autre module. Les modules sont des boîtes noires.
*   La communication entre deux modules différents se fait **UNIQUEMENT** :
    1.  Par des messages asynchrones via le bus d'événements en mémoire (MediatR - `INotification`).
    2.  Par des contrats d'interface publics (ex: `Contracts/`) injectés explicitement via le `ModuleLoader`.

## 5. RÈGLES DE NOMMAGE (UBIQUITOUS LANGUAGE)
*   **Français** : Le cœur métier. Les Entités, Agrégats, Value Objects, Commands, Queries, et Exceptions doivent être en français (ex: `AnnonceEmploi`, `PublierAnnonceCommand`, `CandidatNonAutoriseException`).
*   **Anglais** : La plomberie technique. Les Controllers, Handlers, DbContext, Repositories, et mots-clés d'architecture doivent rester en anglais (ex: `AnnonceController`, `PublierAnnonceCommandHandler`, `ApplicationDbContext`).

## 6. EXPOSITION ET CONTRÔLEURS (CLIENT LAYER)
*   Les Controllers / Minimal APIs de la couche `Client` ne doivent contenir **AUCUNE** logique métier.
*   Leur seul rôle est de parser la requête HTTP, de l'envoyer au bus (MediatR) sous forme de `Command` ou `Query`, et de formater le statut HTTP de retour.
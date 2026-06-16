# Solution + Scaffold 3 Modules Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Créer `CVTech.sln` et scaffolder les 3 modules manquants (CatalogueEmploi, AppelOffreFreelance, ActualiteEtAbonnement) avec domaine + application + infra + tests, pour que `dotnet build CVTech.sln` et `dotnet test CVTech.sln` passent au vert.

**Architecture:** Monolithe modulaire 5-couches par module (Client/Application/Domain/Infrastructure/Loader). Communication inter-modules via MediatR INotification (events AnnoncePubliee, AppelOffrePublie définis dans Contracts/Events/ du module émetteur). ActualiteEtAbonnement référence les .csproj émetteurs pour consommer leurs events.

**Tech Stack:** .NET 10, MediatR 12, FluentValidation 11, EF Core 9 SqlServer, xUnit 2.9, FluentAssertions 6, System.ServiceModel.Syndication (RSS)

---

## File Map

### Solution
- Create: `CVTech.sln`

### Module CatalogueEmploi
- Create: `src/Modules/CatalogueEmploi/CatalogueEmploi.csproj`
- Create: `src/Modules/CatalogueEmploi/Domain/Enums/TypeContrat.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Entites/AnnonceEmploi.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Entites/CurriculumVitae.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Entites/Candidature.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Exceptions/CatalogueEmploiException.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Interfaces/IAnnonceRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Interfaces/ICandidatureRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Contracts/Events/AnnoncePubliee.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PublierAnnonce/PublierAnnonceCommand.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PublierAnnonce/PublierAnnonceCommandHandler.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PostulerAnnonce/PostulerAnnonceCommand.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PostulerAnnonce/PostulerAnnonceCommandHandler.cs`
- Create: `src/Modules/CatalogueEmploi/Infrastructure/Persistence/CatalogueEmploiDbContext.cs`
- Create: `src/Modules/CatalogueEmploi/Infrastructure/Repositories/AnnonceRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Infrastructure/Repositories/CandidatureRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`
- Create: `src/Modules/CatalogueEmploi/Client/DTOs/PublierAnnonceRequest.cs`
- Create: `src/Modules/CatalogueEmploi/Client/DTOs/PostulerAnnonceRequest.cs`
- Create: `src/Modules/CatalogueEmploi/Loader/ModuleLoader.cs`

### Tests CatalogueEmploi
- Create: `tests/CatalogueEmploi.Tests/CatalogueEmploi.Tests.csproj`
- Create: `tests/CatalogueEmploi.Tests/GlobalUsings.cs`
- Create: `tests/CatalogueEmploi.Tests/Fakes/FakeAnnonceRepository.cs`
- Create: `tests/CatalogueEmploi.Tests/Domain/AnnonceEmploiTests.cs`

### Module AppelOffreFreelance
- Create: `src/Modules/AppelOffreFreelance/AppelOffreFreelance.csproj`
- Create: `src/Modules/AppelOffreFreelance/Domain/Entites/AppelOffre.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Entites/PropositionFreelance.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Enums/StatutAppelOffre.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Exceptions/AppelOffreException.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Interfaces/IAppelOffreRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Interfaces/IPropositionRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Contracts/Events/AppelOffrePublie.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/PublierAppelOffre/PublierAppelOffreCommand.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/PublierAppelOffre/PublierAppelOffreCommandHandler.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/SoumettreProposition/SoumettrePropositionCommand.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/SoumettreProposition/SoumettrePropositionCommandHandler.cs`
- Create: `src/Modules/AppelOffreFreelance/Infrastructure/Persistence/AppelOffreDbContext.cs`
- Create: `src/Modules/AppelOffreFreelance/Infrastructure/Repositories/AppelOffreRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Infrastructure/Repositories/PropositionRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`
- Create: `src/Modules/AppelOffreFreelance/Client/DTOs/PublierAppelOffreRequest.cs`
- Create: `src/Modules/AppelOffreFreelance/Client/DTOs/SoumettrePropositionRequest.cs`
- Create: `src/Modules/AppelOffreFreelance/Loader/ModuleLoader.cs`

### Tests AppelOffreFreelance
- Create: `tests/AppelOffreFreelance.Tests/AppelOffreFreelance.Tests.csproj`
- Create: `tests/AppelOffreFreelance.Tests/GlobalUsings.cs`
- Create: `tests/AppelOffreFreelance.Tests/Domain/AppelOffreTests.cs`

### Module ActualiteEtAbonnement
- Create: `src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Entites/ArticleActualite.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Entites/Abonnement.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Entites/Notification.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Enums/CanalDiffusion.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Exceptions/ActualiteException.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IArticleRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IAbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IServiceNotification.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/PublierArticle/PublierArticleCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/PublierArticle/PublierArticleCommandHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/AbonnerDomaine/AbonnerDomaineCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/AbonnerDomaine/AbonnerDomaineCommandHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Handlers/SurAnnoncePublieeHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Handlers/SurAppelOffrePublieHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Persistence/ActualiteDbContext.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/ArticleRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/AbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Services/ServiceNotificationConsole.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Rss/GenerateurRss.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/Controllers/ActualiteController.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/Controllers/AbonnementController.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/PublierArticleRequest.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/AbonnerDomaineRequest.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Loader/ModuleLoader.cs`

### Tests ActualiteEtAbonnement
- Create: `tests/ActualiteEtAbonnement.Tests/ActualiteEtAbonnement.Tests.csproj`
- Create: `tests/ActualiteEtAbonnement.Tests/GlobalUsings.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Domain/ArticleActualiteTests.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Domain/AbonnementTests.cs`

---

## Task 1: Solution CVTech.sln

**Files:**
- Create: `CVTech.sln`

- [ ] **Step 1: Créer la solution et y ajouter les projets existants**

```bash
cd /chemin/vers/ia-skills
dotnet new sln -n CVTech -o .
dotnet sln CVTech.sln add src/Modules/GestionIdentite/GestionIdentite.csproj
dotnet sln CVTech.sln add tests/GestionIdentite.Tests/GestionIdentite.Tests.csproj
```

- [ ] **Step 2: Vérifier que le build passe avec les projets existants**

```bash
dotnet build CVTech.sln
```
Expected: `Build succeeded.` (warnings NU1603 acceptables, 0 erreurs)

- [ ] **Step 3: Commit**

```bash
git add CVTech.sln
git commit -m "chore: add CVTech solution file with GestionIdentite projects"
```

---

## Task 2: Module CatalogueEmploi — .csproj + Domain

**Files:**
- Create: `src/Modules/CatalogueEmploi/CatalogueEmploi.csproj`
- Create: `src/Modules/CatalogueEmploi/Domain/Enums/TypeContrat.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Entites/AnnonceEmploi.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Entites/CurriculumVitae.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Entites/Candidature.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Exceptions/CatalogueEmploiException.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Interfaces/IAnnonceRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Domain/Interfaces/ICandidatureRepository.cs`

- [ ] **Step 1: Créer le .csproj**

`src/Modules/CatalogueEmploi/CatalogueEmploi.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="FluentValidation" Version="11.10.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.5" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="2.2.5" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Créer les enums du domaine**

`src/Modules/CatalogueEmploi/Domain/Enums/TypeContrat.cs`:
```csharp
namespace CVTech.Modules.CatalogueEmploi.Domain.Enums;

public enum TypeContrat
{
    Cdi,
    Cdd,
    Stage,
    Alternance,
    Apprentissage,
}
```

- [ ] **Step 3: Créer les entités du domaine**

`src/Modules/CatalogueEmploi/Domain/Entites/AnnonceEmploi.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Enums;

namespace CVTech.Modules.CatalogueEmploi.Domain.Entites;

public class AnnonceEmploi
{
    public Guid Id { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string DomaineMetier { get; private set; } = string.Empty;
    public TypeContrat TypeContrat { get; private set; }
    public Guid EntrepriseId { get; private set; }
    public bool EstActive { get; private set; } = true;
    public DateTime DatePublication { get; private set; }

    private AnnonceEmploi() { }

    public static AnnonceEmploi Creer(string titre, string description, string domaineMetier, TypeContrat typeContrat, Guid entrepriseId)
    {
        return new AnnonceEmploi
        {
            Id = Guid.NewGuid(),
            Titre = titre,
            Description = description,
            DomaineMetier = domaineMetier,
            TypeContrat = typeContrat,
            EntrepriseId = entrepriseId,
            DatePublication = DateTime.UtcNow,
        };
    }

    public void Desactiver() => EstActive = false;
}
```

`src/Modules/CatalogueEmploi/Domain/Entites/CurriculumVitae.cs`:
```csharp
namespace CVTech.Modules.CatalogueEmploi.Domain.Entites;

public class CurriculumVitae
{
    public Guid Id { get; private set; }
    public Guid CandidatId { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Resume { get; private set; } = string.Empty;
    public DateTime DerniereModification { get; private set; }

    private CurriculumVitae() { }

    public static CurriculumVitae Creer(Guid candidatId, string titre, string resume)
    {
        return new CurriculumVitae
        {
            Id = Guid.NewGuid(),
            CandidatId = candidatId,
            Titre = titre,
            Resume = resume,
            DerniereModification = DateTime.UtcNow,
        };
    }

    public void Modifier(string titre, string resume)
    {
        Titre = titre;
        Resume = resume;
        DerniereModification = DateTime.UtcNow;
    }
}
```

`src/Modules/CatalogueEmploi/Domain/Entites/Candidature.cs`:
```csharp
namespace CVTech.Modules.CatalogueEmploi.Domain.Entites;

public class Candidature
{
    public Guid Id { get; private set; }
    public Guid CandidatId { get; private set; }
    public Guid AnnonceId { get; private set; }
    public Guid CurriculumVitaeId { get; private set; }
    public string? LettreMotivation { get; private set; }
    public DateTime DateDepot { get; private set; }

    private Candidature() { }

    public static Candidature Creer(Guid candidatId, Guid annonceId, Guid curriculumVitaeId, string? lettreMotivation)
    {
        return new Candidature
        {
            Id = Guid.NewGuid(),
            CandidatId = candidatId,
            AnnonceId = annonceId,
            CurriculumVitaeId = curriculumVitaeId,
            LettreMotivation = lettreMotivation,
            DateDepot = DateTime.UtcNow,
        };
    }
}
```

- [ ] **Step 4: Créer l'exception métier**

`src/Modules/CatalogueEmploi/Domain/Exceptions/CatalogueEmploiException.cs`:
```csharp
namespace CVTech.Modules.CatalogueEmploi.Domain.Exceptions;

public class AnnonceIntrouvableException(Guid annonceId)
    : Exception($"Annonce {annonceId} introuvable.");

public class CandidatureDejaExistanteException(Guid candidatId, Guid annonceId)
    : Exception($"Le candidat {candidatId} a déjà postulé à l'annonce {annonceId}.");
```

- [ ] **Step 5: Créer les interfaces repository**

`src/Modules/CatalogueEmploi/Domain/Interfaces/IAnnonceRepository.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;

namespace CVTech.Modules.CatalogueEmploi.Domain.Interfaces;

public interface IAnnonceRepository
{
    Task<AnnonceEmploi?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AjouterAsync(AnnonceEmploi annonce, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnnonceEmploi>> ListerActivesAsync(CancellationToken cancellationToken = default);
}
```

`src/Modules/CatalogueEmploi/Domain/Interfaces/ICandidatureRepository.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;

namespace CVTech.Modules.CatalogueEmploi.Domain.Interfaces;

public interface ICandidatureRepository
{
    Task AjouterAsync(Candidature candidature, CancellationToken cancellationToken = default);
    Task<bool> ExisteDejaAsync(Guid candidatId, Guid annonceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Candidature>> ObtenirParAnnonceAsync(Guid annonceId, CancellationToken cancellationToken = default);
}
```

- [ ] **Step 6: Build pour vérifier**

```bash
dotnet build src/Modules/CatalogueEmploi/CatalogueEmploi.csproj
```
Expected: `Build succeeded.` (0 errors — warnings NU acceptables)

---

## Task 3: Module CatalogueEmploi — Contracts + Application

**Files:**
- Create: `src/Modules/CatalogueEmploi/Contracts/Events/AnnoncePubliee.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PublierAnnonce/PublierAnnonceCommand.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PublierAnnonce/PublierAnnonceCommandHandler.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PostulerAnnonce/PostulerAnnonceCommand.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/PostulerAnnonce/PostulerAnnonceCommandHandler.cs`

- [ ] **Step 1: Créer l'événement domaine**

`src/Modules/CatalogueEmploi/Contracts/Events/AnnoncePubliee.cs`:
```csharp
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Contracts.Events;

public record AnnoncePubliee(Guid AnnonceId, string DomaineMetier, Guid EntrepriseId) : INotification;
```

- [ ] **Step 2: Créer la commande PublierAnnonce**

`src/Modules/CatalogueEmploi/Application/Features/PublierAnnonce/PublierAnnonceCommand.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Enums;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;

public record PublierAnnonceCommand(
    Guid UtilisateurId,
    string Titre,
    string Description,
    string DomaineMetier,
    TypeContrat TypeContrat
) : IRequest<Guid>;
```

`src/Modules/CatalogueEmploi/Application/Features/PublierAnnonce/PublierAnnonceCommandHandler.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Contracts.Events;
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;

public class PublierAnnonceCommandHandler(
    IAnnonceRepository annonceRepository,
    IVerificateurPermission verificateur,
    IPublisher publisher
) : IRequestHandler<PublierAnnonceCommand, Guid>
{
    public async Task<Guid> Handle(PublierAnnonceCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierAnnonceEmploi, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : publication d'annonce.");

        var annonce = AnnonceEmploi.Creer(request.Titre, request.Description, request.DomaineMetier, request.TypeContrat, request.UtilisateurId);
        await annonceRepository.AjouterAsync(annonce, cancellationToken);
        await publisher.Publish(new AnnoncePubliee(annonce.Id, annonce.DomaineMetier, annonce.EntrepriseId), cancellationToken);
        return annonce.Id;
    }
}
```

- [ ] **Step 3: Créer la commande PostulerAnnonce**

`src/Modules/CatalogueEmploi/Application/Features/PostulerAnnonce/PostulerAnnonceCommand.cs`:
```csharp
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;

public record PostulerAnnonceCommand(
    Guid UtilisateurId,
    Guid AnnonceId,
    Guid CurriculumVitaeId,
    string? LettreMotivation
) : IRequest<Guid>;
```

`src/Modules/CatalogueEmploi/Application/Features/PostulerAnnonce/PostulerAnnonceCommandHandler.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Exceptions;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;

public class PostulerAnnonceCommandHandler(
    IAnnonceRepository annonceRepository,
    ICandidatureRepository candidatureRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<PostulerAnnonceCommand, Guid>
{
    public async Task<Guid> Handle(PostulerAnnonceCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PostulerAnnonce, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : candidature.");

        var annonce = await annonceRepository.ObtenirParIdAsync(request.AnnonceId, cancellationToken)
            ?? throw new AnnonceIntrouvableException(request.AnnonceId);

        if (await candidatureRepository.ExisteDejaAsync(request.UtilisateurId, annonce.Id, cancellationToken))
            throw new CandidatureDejaExistanteException(request.UtilisateurId, annonce.Id);

        var candidature = Candidature.Creer(request.UtilisateurId, annonce.Id, request.CurriculumVitaeId, request.LettreMotivation);
        await candidatureRepository.AjouterAsync(candidature, cancellationToken);
        return candidature.Id;
    }
}
```

- [ ] **Step 4: Ajouter la référence GestionIdentite au .csproj**

Dans `src/Modules/CatalogueEmploi/CatalogueEmploi.csproj`, ajouter après `</ItemGroup>`:
```xml
  <ItemGroup>
    <ProjectReference Include="..\GestionIdentite\GestionIdentite.csproj" />
  </ItemGroup>
```

- [ ] **Step 5: Build pour vérifier**

```bash
dotnet build src/Modules/CatalogueEmploi/CatalogueEmploi.csproj
```
Expected: `Build succeeded.`

---

## Task 4: Module CatalogueEmploi — Infrastructure + Client + Loader

**Files:**
- Create: `src/Modules/CatalogueEmploi/Infrastructure/Persistence/CatalogueEmploiDbContext.cs`
- Create: `src/Modules/CatalogueEmploi/Infrastructure/Repositories/AnnonceRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Infrastructure/Repositories/CandidatureRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`
- Create: `src/Modules/CatalogueEmploi/Client/DTOs/PublierAnnonceRequest.cs`
- Create: `src/Modules/CatalogueEmploi/Client/DTOs/PostulerAnnonceRequest.cs`
- Create: `src/Modules/CatalogueEmploi/Loader/ModuleLoader.cs`

- [ ] **Step 1: Créer le DbContext**

`src/Modules/CatalogueEmploi/Infrastructure/Persistence/CatalogueEmploiDbContext.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;

public class CatalogueEmploiDbContext(DbContextOptions<CatalogueEmploiDbContext> options) : DbContext(options)
{
    public DbSet<AnnonceEmploi> Annonces => Set<AnnonceEmploi>();
    public DbSet<CurriculumVitae> CurriculumsVitae => Set<CurriculumVitae>();
    public DbSet<Candidature> Candidatures => Set<Candidature>();
}
```

- [ ] **Step 2: Créer les repositories**

`src/Modules/CatalogueEmploi/Infrastructure/Repositories/AnnonceRepository.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.CatalogueEmploi.Infrastructure.Repositories;

public class AnnonceRepository(CatalogueEmploiDbContext db) : IAnnonceRepository
{
    public async Task<AnnonceEmploi?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await db.Annonces.FindAsync([id], cancellationToken);

    public async Task AjouterAsync(AnnonceEmploi annonce, CancellationToken cancellationToken = default)
    {
        db.Annonces.Add(annonce);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AnnonceEmploi>> ListerActivesAsync(CancellationToken cancellationToken = default)
        => await db.Annonces.Where(a => a.EstActive).ToListAsync(cancellationToken);
}
```

`src/Modules/CatalogueEmploi/Infrastructure/Repositories/CandidatureRepository.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.CatalogueEmploi.Infrastructure.Repositories;

public class CandidatureRepository(CatalogueEmploiDbContext db) : ICandidatureRepository
{
    public async Task AjouterAsync(Candidature candidature, CancellationToken cancellationToken = default)
    {
        db.Candidatures.Add(candidature);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExisteDejaAsync(Guid candidatId, Guid annonceId, CancellationToken cancellationToken = default)
        => await db.Candidatures.AnyAsync(c => c.CandidatId == candidatId && c.AnnonceId == annonceId, cancellationToken);

    public async Task<IReadOnlyList<Candidature>> ObtenirParAnnonceAsync(Guid annonceId, CancellationToken cancellationToken = default)
        => await db.Candidatures.Where(c => c.AnnonceId == annonceId).ToListAsync(cancellationToken);
}
```

- [ ] **Step 3: Créer les DTOs client**

`src/Modules/CatalogueEmploi/Client/DTOs/PublierAnnonceRequest.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Enums;

namespace CVTech.Modules.CatalogueEmploi.Client.DTOs;

public record PublierAnnonceRequest(string Titre, string Description, string DomaineMetier, TypeContrat TypeContrat);
```

`src/Modules/CatalogueEmploi/Client/DTOs/PostulerAnnonceRequest.cs`:
```csharp
namespace CVTech.Modules.CatalogueEmploi.Client.DTOs;

public record PostulerAnnonceRequest(Guid CurriculumVitaeId, string? LettreMotivation);
```

- [ ] **Step 4: Créer le controller**

`src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;
using CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;
using CVTech.Modules.CatalogueEmploi.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.CatalogueEmploi.Client.Controllers;

[ApiController]
[Route("api/annonces")]
public class AnnonceController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierAnnonceCommand(utilisateurId, dto.Titre, dto.Description, dto.DomaineMetier, dto.TypeContrat), ct);
        return CreatedAtAction(nameof(Publier), new { id }, null);
    }

    [HttpPost("{annonceId:guid}/candidatures")]
    public async Task<IActionResult> Postuler(Guid annonceId, [FromBody] PostulerAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PostulerAnnonceCommand(utilisateurId, annonceId, dto.CurriculumVitaeId, dto.LettreMotivation), ct);
        return CreatedAtAction(nameof(Postuler), new { id }, null);
    }
}
```

- [ ] **Step 5: Créer le ModuleLoader**

`src/Modules/CatalogueEmploi/Loader/ModuleLoader.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVTech.Modules.CatalogueEmploi.Loader;

public static class ModuleLoader
{
    public static IServiceCollection AjouterModuleCatalogueEmploi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogueEmploiDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CatalogueEmploi")));

        services.AddScoped<IAnnonceRepository, AnnonceRepository>();
        services.AddScoped<ICandidatureRepository, CandidatureRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}
```

- [ ] **Step 6: Build + ajouter à la solution**

```bash
dotnet build src/Modules/CatalogueEmploi/CatalogueEmploi.csproj
dotnet sln CVTech.sln add src/Modules/CatalogueEmploi/CatalogueEmploi.csproj
```
Expected: `Build succeeded.`

---

## Task 5: Tests CatalogueEmploi (TDD RED → GREEN)

**Files:**
- Create: `tests/CatalogueEmploi.Tests/CatalogueEmploi.Tests.csproj`
- Create: `tests/CatalogueEmploi.Tests/GlobalUsings.cs`
- Create: `tests/CatalogueEmploi.Tests/Fakes/FakeAnnonceRepository.cs`
- Create: `tests/CatalogueEmploi.Tests/Domain/AnnonceEmploiTests.cs`

- [ ] **Step 1: Créer le projet de tests**

`tests/CatalogueEmploi.Tests/CatalogueEmploi.Tests.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="6.12.2" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\Modules\CatalogueEmploi\CatalogueEmploi.csproj" />
  </ItemGroup>
</Project>
```

`tests/CatalogueEmploi.Tests/GlobalUsings.cs`:
```csharp
global using Xunit;
```

- [ ] **Step 2: Créer le fake repository**

`tests/CatalogueEmploi.Tests/Fakes/FakeAnnonceRepository.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;

namespace CatalogueEmploi.Tests.Fakes;

public class FakeAnnonceRepository : IAnnonceRepository
{
    private readonly List<AnnonceEmploi> _annonces = [];

    public void Ajouter(AnnonceEmploi annonce) => _annonces.Add(annonce);

    public Task<AnnonceEmploi?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_annonces.FirstOrDefault(a => a.Id == id));

    public Task AjouterAsync(AnnonceEmploi annonce, CancellationToken cancellationToken = default)
    {
        _annonces.Add(annonce);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AnnonceEmploi>> ListerActivesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AnnonceEmploi>>(_annonces.Where(a => a.EstActive).ToList());
}
```

- [ ] **Step 3: Écrire les tests domaine (RED d'abord)**

`tests/CatalogueEmploi.Tests/Domain/AnnonceEmploiTests.cs`:
```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Enums;
using FluentAssertions;

namespace CatalogueEmploi.Tests.Domain;

public class AnnonceEmploiTests
{
    [Fact]
    public void UneAnnonceEmploi_QuandCreee_EstActiveParDefaut()
    {
        var annonce = AnnonceEmploi.Creer("Dev C#", "Mission intéressante", "dotnet", TypeContrat.Cdi, Guid.NewGuid());

        annonce.EstActive.Should().BeTrue("une annonce créée est active par défaut");
    }

    [Fact]
    public void UneAnnonceEmploi_QuandDesactivee_NestPlusActive()
    {
        var annonce = AnnonceEmploi.Creer("Dev C#", "Mission", "dotnet", TypeContrat.Cdi, Guid.NewGuid());

        annonce.Desactiver();

        annonce.EstActive.Should().BeFalse("une annonce désactivée ne doit plus apparaître");
    }

    [Fact]
    public void UneAnnonceEmploi_QuandCreee_AUneDateDePublicationRecente()
    {
        var avant = DateTime.UtcNow.AddSeconds(-1);
        var annonce = AnnonceEmploi.Creer("Dev C#", "Mission", "dotnet", TypeContrat.Stage, Guid.NewGuid());
        var apres = DateTime.UtcNow.AddSeconds(1);

        annonce.DatePublication.Should().BeAfter(avant).And.BeBefore(apres);
    }
}
```

- [ ] **Step 4: Lancer les tests (doivent être GREEN — domaine pur, pas de dépendances)**

```bash
dotnet test tests/CatalogueEmploi.Tests/CatalogueEmploi.Tests.csproj
```
Expected: `Passed! - Failed: 0, Passed: 3`

- [ ] **Step 5: Ajouter à la solution + commit**

```bash
dotnet sln CVTech.sln add tests/CatalogueEmploi.Tests/CatalogueEmploi.Tests.csproj
git add src/Modules/CatalogueEmploi/ tests/CatalogueEmploi.Tests/
git commit -m "feat: scaffold module CatalogueEmploi with domain, application, infra and tests"
```

---

## Task 6: Module AppelOffreFreelance — .csproj + Domain + Contracts

**Files:**
- Create: `src/Modules/AppelOffreFreelance/AppelOffreFreelance.csproj`
- Create: `src/Modules/AppelOffreFreelance/Domain/Enums/StatutAppelOffre.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Entites/AppelOffre.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Entites/PropositionFreelance.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Exceptions/AppelOffreException.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Interfaces/IAppelOffreRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Domain/Interfaces/IPropositionRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Contracts/Events/AppelOffrePublie.cs`

- [ ] **Step 1: Créer le .csproj**

`src/Modules/AppelOffreFreelance/AppelOffreFreelance.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="FluentValidation" Version="11.10.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.5" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="2.2.5" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\GestionIdentite\GestionIdentite.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Créer les enums + entités**

`src/Modules/AppelOffreFreelance/Domain/Enums/StatutAppelOffre.cs`:
```csharp
namespace CVTech.Modules.AppelOffreFreelance.Domain.Enums;

public enum StatutAppelOffre
{
    Ouvert,
    EnEvaluation,
    Attribue,
    Clos,
}
```

`src/Modules/AppelOffreFreelance/Domain/Entites/AppelOffre.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;

namespace CVTech.Modules.AppelOffreFreelance.Domain.Entites;

public class AppelOffre
{
    public Guid Id { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Contexte { get; private set; } = string.Empty;
    public string DomaineMetier { get; private set; } = string.Empty;
    public decimal BudgetMax { get; private set; }
    public DateTime Deadline { get; private set; }
    public Guid EntrepriseId { get; private set; }
    public StatutAppelOffre Statut { get; private set; } = StatutAppelOffre.Ouvert;
    public DateTime DatePublication { get; private set; }

    private AppelOffre() { }

    public static AppelOffre Creer(string titre, string contexte, string domaineMetier, decimal budgetMax, DateTime deadline, Guid entrepriseId)
    {
        return new AppelOffre
        {
            Id = Guid.NewGuid(),
            Titre = titre,
            Contexte = contexte,
            DomaineMetier = domaineMetier,
            BudgetMax = budgetMax,
            Deadline = deadline,
            EntrepriseId = entrepriseId,
            DatePublication = DateTime.UtcNow,
        };
    }

    public void Clore() => Statut = StatutAppelOffre.Clos;
}
```

`src/Modules/AppelOffreFreelance/Domain/Entites/PropositionFreelance.cs`:
```csharp
namespace CVTech.Modules.AppelOffreFreelance.Domain.Entites;

public class PropositionFreelance
{
    public Guid Id { get; private set; }
    public Guid AppelOffreId { get; private set; }
    public Guid CandidatId { get; private set; }
    public decimal TauxJournalierMoyen { get; private set; }
    public int DureeEstimeeJours { get; private set; }
    public string Methodologie { get; private set; } = string.Empty;
    public DateTime DateSoumission { get; private set; }

    private PropositionFreelance() { }

    public static PropositionFreelance Creer(Guid appelOffreId, Guid candidatId, decimal tjm, int duree, string methodologie)
    {
        return new PropositionFreelance
        {
            Id = Guid.NewGuid(),
            AppelOffreId = appelOffreId,
            CandidatId = candidatId,
            TauxJournalierMoyen = tjm,
            DureeEstimeeJours = duree,
            Methodologie = methodologie,
            DateSoumission = DateTime.UtcNow,
        };
    }
}
```

- [ ] **Step 3: Créer exceptions + interfaces**

`src/Modules/AppelOffreFreelance/Domain/Exceptions/AppelOffreException.cs`:
```csharp
namespace CVTech.Modules.AppelOffreFreelance.Domain.Exceptions;

public class AppelOffreIntrouvableException(Guid id) : Exception($"Appel d'offre {id} introuvable.");
public class AppelOffreClosException(Guid id) : Exception($"L'appel d'offre {id} est clos, les propositions ne sont plus acceptées.");
```

`src/Modules/AppelOffreFreelance/Domain/Interfaces/IAppelOffreRepository.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;

namespace CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;

public interface IAppelOffreRepository
{
    Task<AppelOffre?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AjouterAsync(AppelOffre appelOffre, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppelOffre>> ListerOuvertsAsync(CancellationToken cancellationToken = default);
}
```

`src/Modules/AppelOffreFreelance/Domain/Interfaces/IPropositionRepository.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;

namespace CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;

public interface IPropositionRepository
{
    Task AjouterAsync(PropositionFreelance proposition, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropositionFreelance>> ObtenirParAppelOffreAsync(Guid appelOffreId, CancellationToken cancellationToken = default);
}
```

- [ ] **Step 4: Créer l'événement domaine**

`src/Modules/AppelOffreFreelance/Contracts/Events/AppelOffrePublie.cs`:
```csharp
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Contracts.Events;

public record AppelOffrePublie(Guid AppelOffreId, string DomaineMetier, Guid EntrepriseId) : INotification;
```

---

## Task 7: Module AppelOffreFreelance — Application + Infrastructure + Client + Loader

**Files:**
- Create: `src/Modules/AppelOffreFreelance/Application/Features/PublierAppelOffre/PublierAppelOffreCommand.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/PublierAppelOffre/PublierAppelOffreCommandHandler.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/SoumettreProposition/SoumettrePropositionCommand.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/SoumettreProposition/SoumettrePropositionCommandHandler.cs`
- Create: `src/Modules/AppelOffreFreelance/Infrastructure/Persistence/AppelOffreDbContext.cs`
- Create: `src/Modules/AppelOffreFreelance/Infrastructure/Repositories/AppelOffreRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Infrastructure/Repositories/PropositionRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`
- Create: `src/Modules/AppelOffreFreelance/Client/DTOs/PublierAppelOffreRequest.cs`
- Create: `src/Modules/AppelOffreFreelance/Client/DTOs/SoumettrePropositionRequest.cs`
- Create: `src/Modules/AppelOffreFreelance/Loader/ModuleLoader.cs`

- [ ] **Step 1: Application features**

`src/Modules/AppelOffreFreelance/Application/Features/PublierAppelOffre/PublierAppelOffreCommand.cs`:
```csharp
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;

public record PublierAppelOffreCommand(
    Guid UtilisateurId,
    string Titre,
    string Contexte,
    string DomaineMetier,
    decimal BudgetMax,
    DateTime Deadline
) : IRequest<Guid>;
```

`src/Modules/AppelOffreFreelance/Application/Features/PublierAppelOffre/PublierAppelOffreCommandHandler.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Contracts.Events;
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;

public class PublierAppelOffreCommandHandler(
    IAppelOffreRepository repository,
    IVerificateurPermission verificateur,
    IPublisher publisher
) : IRequestHandler<PublierAppelOffreCommand, Guid>
{
    public async Task<Guid> Handle(PublierAppelOffreCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierAppelOffre, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : publication d'appel d'offre.");

        var ao = AppelOffre.Creer(request.Titre, request.Contexte, request.DomaineMetier, request.BudgetMax, request.Deadline, request.UtilisateurId);
        await repository.AjouterAsync(ao, cancellationToken);
        await publisher.Publish(new AppelOffrePublie(ao.Id, ao.DomaineMetier, ao.EntrepriseId), cancellationToken);
        return ao.Id;
    }
}
```

`src/Modules/AppelOffreFreelance/Application/Features/SoumettreProposition/SoumettrePropositionCommand.cs`:
```csharp
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;

public record SoumettrePropositionCommand(
    Guid UtilisateurId,
    Guid AppelOffreId,
    decimal TauxJournalierMoyen,
    int DureeEstimeeJours,
    string Methodologie
) : IRequest<Guid>;
```

`src/Modules/AppelOffreFreelance/Application/Features/SoumettreProposition/SoumettrePropositionCommandHandler.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;
using CVTech.Modules.AppelOffreFreelance.Domain.Exceptions;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;

public class SoumettrePropositionCommandHandler(
    IAppelOffreRepository appelOffreRepository,
    IPropositionRepository propositionRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<SoumettrePropositionCommand, Guid>
{
    public async Task<Guid> Handle(SoumettrePropositionCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.SoumettrePropositionFreelance, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : soumission de proposition.");

        var ao = await appelOffreRepository.ObtenirParIdAsync(request.AppelOffreId, cancellationToken)
            ?? throw new AppelOffreIntrouvableException(request.AppelOffreId);

        if (ao.Statut == StatutAppelOffre.Clos)
            throw new AppelOffreClosException(ao.Id);

        var proposition = PropositionFreelance.Creer(ao.Id, request.UtilisateurId, request.TauxJournalierMoyen, request.DureeEstimeeJours, request.Methodologie);
        await propositionRepository.AjouterAsync(proposition, cancellationToken);
        return proposition.Id;
    }
}
```

- [ ] **Step 2: Infrastructure**

`src/Modules/AppelOffreFreelance/Infrastructure/Persistence/AppelOffreDbContext.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;

public class AppelOffreDbContext(DbContextOptions<AppelOffreDbContext> options) : DbContext(options)
{
    public DbSet<AppelOffre> AppelsOffre => Set<AppelOffre>();
    public DbSet<PropositionFreelance> Propositions => Set<PropositionFreelance>();
}
```

`src/Modules/AppelOffreFreelance/Infrastructure/Repositories/AppelOffreRepository.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.AppelOffreFreelance.Infrastructure.Repositories;

public class AppelOffreRepository(AppelOffreDbContext db) : IAppelOffreRepository
{
    public async Task<AppelOffre?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await db.AppelsOffre.FindAsync([id], cancellationToken);

    public async Task AjouterAsync(AppelOffre appelOffre, CancellationToken cancellationToken = default)
    {
        db.AppelsOffre.Add(appelOffre);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppelOffre>> ListerOuvertsAsync(CancellationToken cancellationToken = default)
        => await db.AppelsOffre.Where(a => a.Statut == StatutAppelOffre.Ouvert).ToListAsync(cancellationToken);
}
```

`src/Modules/AppelOffreFreelance/Infrastructure/Repositories/PropositionRepository.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.AppelOffreFreelance.Infrastructure.Repositories;

public class PropositionRepository(AppelOffreDbContext db) : IPropositionRepository
{
    public async Task AjouterAsync(PropositionFreelance proposition, CancellationToken cancellationToken = default)
    {
        db.Propositions.Add(proposition);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PropositionFreelance>> ObtenirParAppelOffreAsync(Guid appelOffreId, CancellationToken cancellationToken = default)
        => await db.Propositions.Where(p => p.AppelOffreId == appelOffreId).ToListAsync(cancellationToken);
}
```

- [ ] **Step 3: Client + Loader**

`src/Modules/AppelOffreFreelance/Client/DTOs/PublierAppelOffreRequest.cs`:
```csharp
namespace CVTech.Modules.AppelOffreFreelance.Client.DTOs;

public record PublierAppelOffreRequest(string Titre, string Contexte, string DomaineMetier, decimal BudgetMax, DateTime Deadline);
```

`src/Modules/AppelOffreFreelance/Client/DTOs/SoumettrePropositionRequest.cs`:
```csharp
namespace CVTech.Modules.AppelOffreFreelance.Client.DTOs;

public record SoumettrePropositionRequest(decimal TauxJournalierMoyen, int DureeEstimeeJours, string Methodologie);
```

`src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;
using CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;
using CVTech.Modules.AppelOffreFreelance.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.AppelOffreFreelance.Client.Controllers;

[ApiController]
[Route("api/appels-offre")]
public class AppelOffreController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierAppelOffreRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierAppelOffreCommand(uid, dto.Titre, dto.Contexte, dto.DomaineMetier, dto.BudgetMax, dto.Deadline), ct);
        return CreatedAtAction(nameof(Publier), new { id }, null);
    }

    [HttpPost("{appelOffreId:guid}/propositions")]
    public async Task<IActionResult> SoumettreProposition(Guid appelOffreId, [FromBody] SoumettrePropositionRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new SoumettrePropositionCommand(uid, appelOffreId, dto.TauxJournalierMoyen, dto.DureeEstimeeJours, dto.Methodologie), ct);
        return CreatedAtAction(nameof(SoumettreProposition), new { id }, null);
    }
}
```

`src/Modules/AppelOffreFreelance/Loader/ModuleLoader.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVTech.Modules.AppelOffreFreelance.Loader;

public static class ModuleLoader
{
    public static IServiceCollection AjouterModuleAppelOffreFreelance(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppelOffreDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AppelOffreFreelance")));

        services.AddScoped<IAppelOffreRepository, AppelOffreRepository>();
        services.AddScoped<IPropositionRepository, PropositionRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}
```

- [ ] **Step 4: Build + ajouter à la solution**

```bash
dotnet build src/Modules/AppelOffreFreelance/AppelOffreFreelance.csproj
dotnet sln CVTech.sln add src/Modules/AppelOffreFreelance/AppelOffreFreelance.csproj
```
Expected: `Build succeeded.`

---

## Task 8: Tests AppelOffreFreelance (TDD RED → GREEN)

**Files:**
- Create: `tests/AppelOffreFreelance.Tests/AppelOffreFreelance.Tests.csproj`
- Create: `tests/AppelOffreFreelance.Tests/GlobalUsings.cs`
- Create: `tests/AppelOffreFreelance.Tests/Domain/AppelOffreTests.cs`

- [ ] **Step 1: Créer le projet de tests**

`tests/AppelOffreFreelance.Tests/AppelOffreFreelance.Tests.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="6.12.2" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\Modules\AppelOffreFreelance\AppelOffreFreelance.csproj" />
  </ItemGroup>
</Project>
```

`tests/AppelOffreFreelance.Tests/GlobalUsings.cs`:
```csharp
global using Xunit;
```

- [ ] **Step 2: Écrire les tests domaine**

`tests/AppelOffreFreelance.Tests/Domain/AppelOffreTests.cs`:
```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;
using FluentAssertions;

namespace AppelOffreFreelance.Tests.Domain;

public class AppelOffreTests
{
    [Fact]
    public void UnAppelOffre_QuandCree_ALeStatutOuvert()
    {
        var ao = AppelOffre.Creer("Mission Cloud", "Contexte", "cloud-azure", 50000m, DateTime.UtcNow.AddMonths(1), Guid.NewGuid());

        ao.Statut.Should().Be(StatutAppelOffre.Ouvert, "un appel d'offre nouvellement créé est ouvert");
    }

    [Fact]
    public void UnAppelOffre_QuandClos_NAcceptePlusDePropositions()
    {
        var ao = AppelOffre.Creer("Mission Cloud", "Contexte", "cloud-azure", 50000m, DateTime.UtcNow.AddMonths(1), Guid.NewGuid());

        ao.Clore();

        ao.Statut.Should().Be(StatutAppelOffre.Clos, "un appel d'offre clos ne peut plus recevoir de propositions");
    }

    [Fact]
    public void UnePropositionFreelance_QuandCreee_ConserveLesTjmEtDuree()
    {
        var proposition = PropositionFreelance.Creer(Guid.NewGuid(), Guid.NewGuid(), 650m, 20, "Méthode agile");

        proposition.TauxJournalierMoyen.Should().Be(650m);
        proposition.DureeEstimeeJours.Should().Be(20);
    }
}
```

- [ ] **Step 3: Lancer les tests**

```bash
dotnet test tests/AppelOffreFreelance.Tests/AppelOffreFreelance.Tests.csproj
```
Expected: `Passed! - Failed: 0, Passed: 3`

- [ ] **Step 4: Ajouter à la solution + commit**

```bash
dotnet sln CVTech.sln add tests/AppelOffreFreelance.Tests/AppelOffreFreelance.Tests.csproj
git add src/Modules/AppelOffreFreelance/ tests/AppelOffreFreelance.Tests/
git commit -m "feat: scaffold module AppelOffreFreelance with domain, application, infra and tests"
```

---

## Task 9: Module ActualiteEtAbonnement — .csproj + Domain + Contracts

**Files:**
- Create: `src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Enums/CanalDiffusion.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Entites/ArticleActualite.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Entites/Abonnement.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Entites/Notification.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Exceptions/ActualiteException.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IArticleRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IAbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IServiceNotification.cs`

- [ ] **Step 1: Créer le .csproj (référence les deux modules émetteurs d'events)**

`src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="FluentValidation" Version="11.10.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.5" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="2.2.5" />
    <PackageReference Include="System.ServiceModel.Syndication" Version="9.0.5" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\GestionIdentite\GestionIdentite.csproj" />
    <ProjectReference Include="..\CatalogueEmploi\CatalogueEmploi.csproj" />
    <ProjectReference Include="..\AppelOffreFreelance\AppelOffreFreelance.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Créer enums + entités domaine**

`src/Modules/ActualiteEtAbonnement/Domain/Enums/CanalDiffusion.cs`:
```csharp
namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;

public enum CanalDiffusion
{
    Email,
    InApp,
}
```

`src/Modules/ActualiteEtAbonnement/Domain/Entites/ArticleActualite.cs`:
```csharp
namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

public class ArticleActualite
{
    public Guid Id { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Contenu { get; private set; } = string.Empty;
    public string? DomaineMetier { get; private set; }
    public Guid AuteurId { get; private set; }
    public DateTime DatePublication { get; private set; }

    private ArticleActualite() { }

    public static ArticleActualite Creer(string titre, string contenu, Guid auteurId, string? domaineMetier = null)
    {
        return new ArticleActualite
        {
            Id = Guid.NewGuid(),
            Titre = titre,
            Contenu = contenu,
            DomaineMetier = domaineMetier,
            AuteurId = auteurId,
            DatePublication = DateTime.UtcNow,
        };
    }
}
```

`src/Modules/ActualiteEtAbonnement/Domain/Entites/Abonnement.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

public class Abonnement
{
    public Guid Id { get; private set; }
    public Guid UtilisateurId { get; private set; }
    public string DomaineMetier { get; private set; } = string.Empty;
    public CanalDiffusion Canal { get; private set; }
    public DateTime DateAbonnement { get; private set; }

    private Abonnement() { }

    public static Abonnement Creer(Guid utilisateurId, string domaineMetier, CanalDiffusion canal)
    {
        return new Abonnement
        {
            Id = Guid.NewGuid(),
            UtilisateurId = utilisateurId,
            DomaineMetier = domaineMetier,
            Canal = canal,
            DateAbonnement = DateTime.UtcNow,
        };
    }
}
```

`src/Modules/ActualiteEtAbonnement/Domain/Entites/Notification.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid UtilisateurId { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Corps { get; private set; } = string.Empty;
    public CanalDiffusion Canal { get; private set; }
    public bool EstLue { get; private set; }
    public DateTime DateEnvoi { get; private set; }

    private Notification() { }

    public static Notification Creer(Guid utilisateurId, string titre, string corps, CanalDiffusion canal)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UtilisateurId = utilisateurId,
            Titre = titre,
            Corps = corps,
            Canal = canal,
            DateEnvoi = DateTime.UtcNow,
        };
    }

    public void MarquerCommeLue() => EstLue = true;
}
```

- [ ] **Step 3: Exceptions + interfaces**

`src/Modules/ActualiteEtAbonnement/Domain/Exceptions/ActualiteException.cs`:
```csharp
namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;

public class ArticleIntrouvableException(Guid id) : Exception($"Article {id} introuvable.");
public class AbonnementDejaExistantException(Guid utilisateurId, string domaine)
    : Exception($"L'utilisateur {utilisateurId} est déjà abonné au domaine '{domaine}'.");
```

`src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IArticleRepository.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface IArticleRepository
{
    Task AjouterAsync(ArticleActualite article, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArticleActualite>> ListerAsync(string? domaineMetier = null, CancellationToken cancellationToken = default);
}
```

`src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IAbonnementRepository.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface IAbonnementRepository
{
    Task AjouterAsync(Abonnement abonnement, CancellationToken cancellationToken = default);
    Task<bool> ExisteDejaAsync(Guid utilisateurId, string domaineMetier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Abonnement>> ObtenirParDomaineAsync(string domaineMetier, CancellationToken cancellationToken = default);
}
```

`src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IServiceNotification.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface IServiceNotification
{
    Task EnvoyerAsync(Abonnement abonnement, string titre, string corps, CancellationToken cancellationToken = default);
}
```

---

## Task 10: Module ActualiteEtAbonnement — Application + Infrastructure + Client + Loader

**Files:**
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/PublierArticle/PublierArticleCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/PublierArticle/PublierArticleCommandHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/AbonnerDomaine/AbonnerDomaineCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/AbonnerDomaine/AbonnerDomaineCommandHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Handlers/SurAnnoncePublieeHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Handlers/SurAppelOffrePublieHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Persistence/ActualiteDbContext.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/ArticleRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/AbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Services/ServiceNotificationConsole.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Rss/GenerateurRss.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/Controllers/ActualiteController.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/Controllers/AbonnementController.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/PublierArticleRequest.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/AbonnerDomaineRequest.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Loader/ModuleLoader.cs`

- [ ] **Step 1: Application — commandes**

`src/Modules/ActualiteEtAbonnement/Application/Features/PublierArticle/PublierArticleCommand.cs`:
```csharp
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.PublierArticle;

public record PublierArticleCommand(
    Guid UtilisateurId,
    string Titre,
    string Contenu,
    string? DomaineMetier
) : IRequest<Guid>;
```

`src/Modules/ActualiteEtAbonnement/Application/Features/PublierArticle/PublierArticleCommandHandler.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.PublierArticle;

public class PublierArticleCommandHandler(
    IArticleRepository articleRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<PublierArticleCommand, Guid>
{
    public async Task<Guid> Handle(PublierArticleCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierArticleActualite, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : publication d'article.");

        var article = ArticleActualite.Creer(request.Titre, request.Contenu, request.UtilisateurId, request.DomaineMetier);
        await articleRepository.AjouterAsync(article, cancellationToken);
        return article.Id;
    }
}
```

`src/Modules/ActualiteEtAbonnement/Application/Features/AbonnerDomaine/AbonnerDomaineCommand.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;

public record AbonnerDomaineCommand(
    Guid UtilisateurId,
    string DomaineMetier,
    CanalDiffusion Canal
) : IRequest;
```

`src/Modules/ActualiteEtAbonnement/Application/Features/AbonnerDomaine/AbonnerDomaineCommandHandler.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;

public class AbonnerDomaineCommandHandler(
    IAbonnementRepository abonnementRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<AbonnerDomaineCommand>
{
    public async Task Handle(AbonnerDomaineCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.SAbonnerDomaineMetier, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : abonnement domaine.");

        if (await abonnementRepository.ExisteDejaAsync(request.UtilisateurId, request.DomaineMetier, cancellationToken))
            throw new AbonnementDejaExistantException(request.UtilisateurId, request.DomaineMetier);

        var abonnement = Abonnement.Creer(request.UtilisateurId, request.DomaineMetier, request.Canal);
        await abonnementRepository.AjouterAsync(abonnement, cancellationToken);
    }
}
```

- [ ] **Step 2: Application — handlers événements inter-modules**

`src/Modules/ActualiteEtAbonnement/Application/Handlers/SurAnnoncePublieeHandler.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Contracts.Events;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Handlers;

public class SurAnnoncePublieeHandler(
    IAbonnementRepository abonnementRepository,
    IServiceNotification serviceNotification
) : INotificationHandler<AnnoncePubliee>
{
    public async Task Handle(AnnoncePubliee notification, CancellationToken cancellationToken)
    {
        var abonnes = await abonnementRepository.ObtenirParDomaineAsync(notification.DomaineMetier, cancellationToken);
        foreach (var abonne in abonnes)
        {
            await serviceNotification.EnvoyerAsync(
                abonne,
                $"Nouvelle annonce dans le domaine {notification.DomaineMetier}",
                $"Une nouvelle annonce d'emploi vient d'être publiée.",
                cancellationToken);
        }
    }
}
```

`src/Modules/ActualiteEtAbonnement/Application/Handlers/SurAppelOffrePublieHandler.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Contracts.Events;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Handlers;

public class SurAppelOffrePublieHandler(
    IAbonnementRepository abonnementRepository,
    IServiceNotification serviceNotification
) : INotificationHandler<AppelOffrePublie>
{
    public async Task Handle(AppelOffrePublie notification, CancellationToken cancellationToken)
    {
        var abonnes = await abonnementRepository.ObtenirParDomaineAsync(notification.DomaineMetier, cancellationToken);
        foreach (var abonne in abonnes)
        {
            await serviceNotification.EnvoyerAsync(
                abonne,
                $"Nouvel appel d'offre dans le domaine {notification.DomaineMetier}",
                $"Un nouvel appel d'offre freelance vient d'être publié.",
                cancellationToken);
        }
    }
}
```

- [ ] **Step 3: Infrastructure**

`src/Modules/ActualiteEtAbonnement/Infrastructure/Persistence/ActualiteDbContext.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;

public class ActualiteDbContext(DbContextOptions<ActualiteDbContext> options) : DbContext(options)
{
    public DbSet<ArticleActualite> Articles => Set<ArticleActualite>();
    public DbSet<Abonnement> Abonnements => Set<Abonnement>();
    public DbSet<Notification> Notifications => Set<Notification>();
}
```

`src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/ArticleRepository.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;

public class ArticleRepository(ActualiteDbContext db) : IArticleRepository
{
    public async Task AjouterAsync(ArticleActualite article, CancellationToken cancellationToken = default)
    {
        db.Articles.Add(article);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ArticleActualite>> ListerAsync(string? domaineMetier = null, CancellationToken cancellationToken = default)
    {
        var query = db.Articles.AsQueryable();
        if (domaineMetier is not null)
            query = query.Where(a => a.DomaineMetier == domaineMetier);
        return await query.OrderByDescending(a => a.DatePublication).ToListAsync(cancellationToken);
    }
}
```

`src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/AbonnementRepository.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;

public class AbonnementRepository(ActualiteDbContext db) : IAbonnementRepository
{
    public async Task AjouterAsync(Abonnement abonnement, CancellationToken cancellationToken = default)
    {
        db.Abonnements.Add(abonnement);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExisteDejaAsync(Guid utilisateurId, string domaineMetier, CancellationToken cancellationToken = default)
        => await db.Abonnements.AnyAsync(a => a.UtilisateurId == utilisateurId && a.DomaineMetier == domaineMetier, cancellationToken);

    public async Task<IReadOnlyList<Abonnement>> ObtenirParDomaineAsync(string domaineMetier, CancellationToken cancellationToken = default)
        => await db.Abonnements.Where(a => a.DomaineMetier == domaineMetier).ToListAsync(cancellationToken);
}
```

`src/Modules/ActualiteEtAbonnement/Infrastructure/Services/ServiceNotificationConsole.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;

public class ServiceNotificationConsole(ILogger<ServiceNotificationConsole> logger) : IServiceNotification
{
    public Task EnvoyerAsync(Abonnement abonnement, string titre, string corps, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION][{Canal}] → Utilisateur {UserId} : {Titre} — {Corps}",
            abonnement.Canal, abonnement.UtilisateurId, titre, corps);
        return Task.CompletedTask;
    }
}
```

`src/Modules/ActualiteEtAbonnement/Infrastructure/Rss/GenerateurRss.cs`:
```csharp
using System.ServiceModel.Syndication;
using System.Xml;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Rss;

public static class GenerateurRss
{
    public static string GenererFlux(IReadOnlyList<ArticleActualite> articles, string baseUrl)
    {
        var feed = new SyndicationFeed(
            "CVTech Actualités",
            "Fil d'actualité éditorial de la plateforme CVTech",
            new Uri(baseUrl))
        {
            Items = articles.Select(a => new SyndicationItem(
                a.Titre,
                a.Contenu,
                new Uri($"{baseUrl}/articles/{a.Id}"),
                a.Id.ToString(),
                a.DatePublication))
        };

        using var sw = new StringWriter();
        using var writer = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true });
        new Rss20FeedFormatter(feed).WriteTo(writer);
        writer.Flush();
        return sw.ToString();
    }
}
```

- [ ] **Step 4: Client**

`src/Modules/ActualiteEtAbonnement/Client/DTOs/PublierArticleRequest.cs`:
```csharp
namespace CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;

public record PublierArticleRequest(string Titre, string Contenu, string? DomaineMetier);
```

`src/Modules/ActualiteEtAbonnement/Client/DTOs/AbonnerDomaineRequest.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;

public record AbonnerDomaineRequest(string DomaineMetier, CanalDiffusion Canal);
```

`src/Modules/ActualiteEtAbonnement/Client/Controllers/ActualiteController.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.PublierArticle;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Rss;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Route("api/actualites")]
public class ActualiteController(ISender sender, IArticleRepository articleRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierArticleRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierArticleCommand(uid, dto.Titre, dto.Contenu, dto.DomaineMetier), ct);
        return CreatedAtAction(nameof(Publier), new { id }, null);
    }

    [HttpGet("/feed/rss")]
    public async Task<IActionResult> Rss([FromQuery] string? domaine, CancellationToken ct)
    {
        var articles = await articleRepository.ListerAsync(domaine, ct);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var xml = GenerateurRss.GenererFlux(articles, baseUrl);
        return Content(xml, "application/rss+xml; charset=utf-8");
    }
}
```

`src/Modules/ActualiteEtAbonnement/Client/Controllers/AbonnementController.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Route("api/abonnements")]
public class AbonnementController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Abonner([FromBody] AbonnerDomaineRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new AbonnerDomaineCommand(uid, dto.DomaineMetier, dto.Canal), ct);
        return NoContent();
    }
}
```

- [ ] **Step 5: Loader**

`src/Modules/ActualiteEtAbonnement/Loader/ModuleLoader.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVTech.Modules.ActualiteEtAbonnement.Loader;

public static class ModuleLoader
{
    public static IServiceCollection AjouterModuleActualiteEtAbonnement(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ActualiteDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ActualiteEtAbonnement")));

        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IAbonnementRepository, AbonnementRepository>();
        services.AddScoped<IServiceNotification, ServiceNotificationConsole>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}
```

- [ ] **Step 6: Build + ajouter à la solution**

```bash
dotnet build src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj
dotnet sln CVTech.sln add src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj
```
Expected: `Build succeeded.`

---

## Task 11: Tests ActualiteEtAbonnement (TDD RED → GREEN)

**Files:**
- Create: `tests/ActualiteEtAbonnement.Tests/ActualiteEtAbonnement.Tests.csproj`
- Create: `tests/ActualiteEtAbonnement.Tests/GlobalUsings.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Domain/ArticleActualiteTests.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Domain/AbonnementTests.cs`

- [ ] **Step 1: Créer le projet de tests**

`tests/ActualiteEtAbonnement.Tests/ActualiteEtAbonnement.Tests.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="6.12.2" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\Modules\ActualiteEtAbonnement\ActualiteEtAbonnement.csproj" />
  </ItemGroup>
</Project>
```

`tests/ActualiteEtAbonnement.Tests/GlobalUsings.cs`:
```csharp
global using Xunit;
```

- [ ] **Step 2: Écrire les tests domaine**

`tests/ActualiteEtAbonnement.Tests/Domain/ArticleActualiteTests.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using FluentAssertions;

namespace ActualiteEtAbonnement.Tests.Domain;

public class ArticleActualiteTests
{
    [Fact]
    public void UnArticle_QuandCree_AUneDateDePublicationRecente()
    {
        var avant = DateTime.UtcNow.AddSeconds(-1);
        var article = ArticleActualite.Creer("Titre test", "Contenu test", Guid.NewGuid());
        var apres = DateTime.UtcNow.AddSeconds(1);

        article.DatePublication.Should().BeAfter(avant).And.BeBefore(apres);
    }

    [Fact]
    public void UnArticle_QuandCreeAvecDomaine_ConserveLeDomaineMetier()
    {
        var article = ArticleActualite.Creer("Cloud Azure 2026", "Contenu", Guid.NewGuid(), "cloud-azure");

        article.DomaineMetier.Should().Be("cloud-azure");
    }

    [Fact]
    public void UnArticle_QuandCreeSansDomaine_AUnDomaineNull()
    {
        var article = ArticleActualite.Creer("Actu générale", "Contenu", Guid.NewGuid());

        article.DomaineMetier.Should().BeNull("un article sans domaine cible tous les lecteurs");
    }
}
```

`tests/ActualiteEtAbonnement.Tests/Domain/AbonnementTests.cs`:
```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using FluentAssertions;

namespace ActualiteEtAbonnement.Tests.Domain;

public class AbonnementTests
{
    [Fact]
    public void UnAbonnement_QuandCree_LieUtilisateurAuDomaine()
    {
        var utilisateurId = Guid.NewGuid();
        var abonnement = Abonnement.Creer(utilisateurId, "cloud-azure", CanalDiffusion.Email);

        abonnement.UtilisateurId.Should().Be(utilisateurId);
        abonnement.DomaineMetier.Should().Be("cloud-azure");
    }

    [Fact]
    public void UneNotification_QuandCreee_EstNonLueParDefaut()
    {
        var notif = Notification.Creer(Guid.NewGuid(), "Nouvelle annonce", "Contenu", CanalDiffusion.InApp);

        notif.EstLue.Should().BeFalse("une notification fraîche n'a pas encore été lue");
    }

    [Fact]
    public void UneNotification_QuandMarqueeCommeLue_EstLue()
    {
        var notif = Notification.Creer(Guid.NewGuid(), "Nouvelle annonce", "Contenu", CanalDiffusion.Email);

        notif.MarquerCommeLue();

        notif.EstLue.Should().BeTrue();
    }
}
```

- [ ] **Step 3: Lancer les tests**

```bash
dotnet test tests/ActualiteEtAbonnement.Tests/ActualiteEtAbonnement.Tests.csproj
```
Expected: `Passed! - Failed: 0, Passed: 6`

- [ ] **Step 4: Ajouter à la solution + commit**

```bash
dotnet sln CVTech.sln add tests/ActualiteEtAbonnement.Tests/ActualiteEtAbonnement.Tests.csproj
git add src/Modules/ActualiteEtAbonnement/ tests/ActualiteEtAbonnement.Tests/
git commit -m "feat: scaffold module ActualiteEtAbonnement with domain, application, infra, RSS and tests"
```

---

## Task 12: Validation finale — build + test solution complète

- [ ] **Step 1: Build solution entière**

```bash
dotnet build CVTech.sln
```
Expected: `Build succeeded.` — 0 erreurs (warnings NU1603 sur versions mineures acceptables)

- [ ] **Step 2: Lancer tous les tests**

```bash
dotnet test CVTech.sln
```
Expected: `Passed! - Failed: 0, Passed: 20` (14 GestionIdentite + 3 Catalogue + 3 AppelOffre + 6 Actualite — total: 26)

- [ ] **Step 3: Vérifier que tous les projets sont dans la solution**

```bash
dotnet sln CVTech.sln list
```
Expected: 7 projets listés (4 modules src + 4 tests — note: GestionIdentite et son test project déjà présents dès Task 1)

- [ ] **Step 4: Commit final**

```bash
git add CVTech.sln
git commit -m "chore: add all modules and test projects to CVTech solution"
```

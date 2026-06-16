# Abonnement Testable End-to-End — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Complete the subscription notification pipeline so the full flow (subscribe → publish annonce → notification persisted → UI shows subscriptions) is testable.

**Architecture:** Add `INotificationRepository` + EF Core impl, replace `ServiceNotificationConsole` with `ServiceNotificationDb` (persists InApp to DB, logs Email to console), add GET/DELETE on abonnements and GET/PATCH on notifications, update the frontend abonnements page to list + delete subscriptions.

**Tech Stack:** .NET 10, EF Core, MediatR 12, xunit + FluentAssertions, Next.js (use `'use client'` + `useState` + `apiFetch` patterns from existing pages)

---

## File Map

**Domain**
- Modify: `src/Modules/ActualiteEtAbonnement/Domain/Exceptions/ActualiteException.cs`
- Modify: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IAbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/INotificationRepository.cs`

**Infrastructure**
- Modify: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/AbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/NotificationRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Services/ServiceNotificationDb.cs`
- Modify: `src/Modules/ActualiteEtAbonnement/Loader/ModuleLoader.cs`

**Application**
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirAbonnements/ObtenirAbonnementsQuery.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirAbonnements/ObtenirAbonnementsQueryHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/SupprimerAbonnement/SupprimerAbonnementCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/SupprimerAbonnement/SupprimerAbonnementCommandHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirNotifications/ObtenirNotificationsQuery.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirNotifications/ObtenirNotificationsQueryHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/MarquerNotificationLue/MarquerNotificationLueCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/MarquerNotificationLue/MarquerNotificationLueCommandHandler.cs`

**API**
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/AbonnementDto.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/NotificationDto.cs`
- Modify: `src/Modules/ActualiteEtAbonnement/Client/Controllers/AbonnementController.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/Controllers/NotificationsController.cs`

**Tests**
- Create: `tests/ActualiteEtAbonnement.Tests/Domain/NotificationTests.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Application/SupprimerAbonnementTests.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Application/ServiceNotificationDbTests.cs`

**Frontend**
- Modify: `frontend/app/candidat/abonnements/page.tsx`

---

## Task 1 — Domain: exception + interfaces

**Files:**
- Modify: `src/Modules/ActualiteEtAbonnement/Domain/Exceptions/ActualiteException.cs`
- Modify: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IAbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/INotificationRepository.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Domain/NotificationTests.cs`

- [ ] **Step 1: Write the failing test**

Create `tests/ActualiteEtAbonnement.Tests/Domain/NotificationTests.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using FluentAssertions;

namespace ActualiteEtAbonnement.Tests.Domain;

public class NotificationTests
{
    [Fact]
    public void UneNotification_QuandCreee_ALesBonnesValeurs()
    {
        var userId = Guid.NewGuid();
        var notif = Notification.Creer(userId, "Titre", "Corps", CanalDiffusion.InApp);

        notif.UtilisateurId.Should().Be(userId);
        notif.Titre.Should().Be("Titre");
        notif.Corps.Should().Be("Corps");
        notif.Canal.Should().Be(CanalDiffusion.InApp);
        notif.EstLue.Should().BeFalse();
    }

    [Fact]
    public void UneNotification_QuandMarqueeCommeLue_EstLueEstTrue()
    {
        var notif = Notification.Creer(Guid.NewGuid(), "T", "C", CanalDiffusion.InApp);
        notif.MarquerCommeLue();
        notif.EstLue.Should().BeTrue();
    }
}
```

- [ ] **Step 2: Run to verify it fails**

```bash
dotnet test tests/ActualiteEtAbonnement.Tests --filter "NotificationTests" -v minimal
```

Expected: PASS (these test existing domain code — confirms the domain entity works before we add plumbing)

- [ ] **Step 3: Add `AbonnementNonTrouveException` to exceptions file**

Replace the contents of `src/Modules/ActualiteEtAbonnement/Domain/Exceptions/ActualiteException.cs`:

```csharp
namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;

public class ArticleIntrouvableException(Guid id) : Exception($"Article {id} introuvable.");
public class AbonnementDejaExistantException(Guid utilisateurId, string domaine)
    : Exception($"L'utilisateur {utilisateurId} est déjà abonné au domaine '{domaine}'.");
public class AbonnementNonTrouveException(Guid id)
    : Exception($"Abonnement {id} introuvable ou non autorisé.");
public class NotificationNonTrouveException(Guid id)
    : Exception($"Notification {id} introuvable ou non autorisée.");
```

- [ ] **Step 4: Extend `IAbonnementRepository`**

Replace the contents of `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/IAbonnementRepository.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface IAbonnementRepository
{
    Task AjouterAsync(Abonnement abonnement, CancellationToken cancellationToken = default);
    Task<bool> ExisteDejaAsync(Guid utilisateurId, string domaineMetier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Abonnement>> ObtenirParDomaineAsync(string domaineMetier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Abonnement>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default);
    Task SupprimerAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default);
}
```

- [ ] **Step 5: Create `INotificationRepository`**

Create `src/Modules/ActualiteEtAbonnement/Domain/Interfaces/INotificationRepository.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface INotificationRepository
{
    Task AjouterAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default);
    Task MarquerCommeLueAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default);
}
```

- [ ] **Step 6: Build to verify no errors**

```bash
dotnet build src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj -v minimal 2>&1 | tail -5
```

Expected: `Build succeeded.` — `AbonnementRepository` will have a compile error about missing methods; that's fine and expected.

- [ ] **Step 7: Commit**

```bash
git add src/Modules/ActualiteEtAbonnement/Domain/ tests/ActualiteEtAbonnement.Tests/Domain/NotificationTests.cs
git commit -m "feat(abonnement): extend domain interfaces and add missing exceptions"
```

---

## Task 2 — Infrastructure: repositories + ServiceNotificationDb + ModuleLoader

**Files:**
- Modify: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/AbonnementRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/NotificationRepository.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Infrastructure/Services/ServiceNotificationDb.cs`
- Modify: `src/Modules/ActualiteEtAbonnement/Loader/ModuleLoader.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Application/ServiceNotificationDbTests.cs`

- [ ] **Step 1: Write the failing test for ServiceNotificationDb**

Create `tests/ActualiteEtAbonnement.Tests/Application/ServiceNotificationDbTests.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ActualiteEtAbonnement.Tests.Application;

public class ServiceNotificationDbTests
{
    private sealed class FakeNotificationRepository : INotificationRepository
    {
        public List<Notification> Saved { get; } = [];

        public Task AjouterAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            Saved.Add(notification);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Notification>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Notification>>(Saved.Where(n => n.UtilisateurId == utilisateurId).ToList());

        public Task MarquerCommeLueAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
        {
            var notif = Saved.FirstOrDefault(n => n.Id == id && n.UtilisateurId == utilisateurId);
            notif?.MarquerCommeLue();
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Envoyer_QuandCanal_InApp_PersisteLaNotification()
    {
        var repo = new FakeNotificationRepository();
        var service = new ServiceNotificationDb(repo, NullLogger<ServiceNotificationDb>.Instance);
        var abonnement = Abonnement.Creer(Guid.NewGuid(), "cloud", CanalDiffusion.InApp);

        await service.EnvoyerAsync(abonnement, "Titre", "Corps");

        repo.Saved.Should().HaveCount(1);
        repo.Saved[0].UtilisateurId.Should().Be(abonnement.UtilisateurId);
        repo.Saved[0].Canal.Should().Be(CanalDiffusion.InApp);
        repo.Saved[0].EstLue.Should().BeFalse();
    }

    [Fact]
    public async Task Envoyer_QuandCanal_Email_PersisteLaNotification()
    {
        var repo = new FakeNotificationRepository();
        var service = new ServiceNotificationDb(repo, NullLogger<ServiceNotificationDb>.Instance);
        var abonnement = Abonnement.Creer(Guid.NewGuid(), "cloud", CanalDiffusion.Email);

        await service.EnvoyerAsync(abonnement, "Titre", "Corps");

        repo.Saved.Should().HaveCount(1);
        repo.Saved[0].Canal.Should().Be(CanalDiffusion.Email);
    }
}
```

- [ ] **Step 2: Run to verify it fails**

```bash
dotnet test tests/ActualiteEtAbonnement.Tests --filter "ServiceNotificationDbTests" -v minimal
```

Expected: compile error — `ServiceNotificationDb` doesn't have this constructor yet.

- [ ] **Step 3: Implement `AbonnementRepository` new methods**

Replace the full contents of `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/AbonnementRepository.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
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

    public async Task<IReadOnlyList<Abonnement>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
        => await db.Abonnements.Where(a => a.UtilisateurId == utilisateurId).ToListAsync(cancellationToken);

    public async Task SupprimerAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var abonnement = await db.Abonnements
            .FirstOrDefaultAsync(a => a.Id == id && a.UtilisateurId == utilisateurId, cancellationToken)
            ?? throw new AbonnementNonTrouveException(id);
        db.Abonnements.Remove(abonnement);
        await db.SaveChangesAsync(cancellationToken);
    }
}
```

- [ ] **Step 4: Create `NotificationRepository`**

Create `src/Modules/ActualiteEtAbonnement/Infrastructure/Repositories/NotificationRepository.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;

public class NotificationRepository(ActualiteDbContext db) : INotificationRepository
{
    public async Task AjouterAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        db.Notifications.Add(notification);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
        => await db.Notifications
            .Where(n => n.UtilisateurId == utilisateurId)
            .OrderByDescending(n => n.DateEnvoi)
            .ToListAsync(cancellationToken);

    public async Task MarquerCommeLueAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var notif = await db.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UtilisateurId == utilisateurId, cancellationToken)
            ?? throw new NotificationNonTrouveException(id);
        notif.MarquerCommeLue();
        await db.SaveChangesAsync(cancellationToken);
    }
}
```

- [ ] **Step 5: Create `ServiceNotificationDb`**

Create `src/Modules/ActualiteEtAbonnement/Infrastructure/Services/ServiceNotificationDb.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;

public class ServiceNotificationDb(
    INotificationRepository notificationRepository,
    ILogger<ServiceNotificationDb> logger) : IServiceNotification
{
    public async Task EnvoyerAsync(Abonnement abonnement, string titre, string corps, CancellationToken cancellationToken = default)
    {
        var notification = Notification.Creer(abonnement.UtilisateurId, titre, corps, abonnement.Canal);
        await notificationRepository.AjouterAsync(notification, cancellationToken);

        if (abonnement.Canal == CanalDiffusion.Email)
            logger.LogInformation("[EMAIL SIMULÉ] → {UserId} : {Titre}", abonnement.UtilisateurId, titre);
    }
}
```

- [ ] **Step 6: Update `ModuleLoader` to register new services**

Replace the contents of `src/Modules/ActualiteEtAbonnement/Loader/ModuleLoader.cs`:

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
    public static IServiceCollection AjouterModuleActualiteEtAbonnement(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? configureDb = null)
    {
        services.AddDbContext<ActualiteDbContext>(options =>
        {
            if (configureDb != null) configureDb(options);
            else options.UseSqlServer(configuration.GetConnectionString("ActualiteEtAbonnement"));
        });

        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IAbonnementRepository, AbonnementRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IServiceNotification, ServiceNotificationDb>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}
```

- [ ] **Step 7: Run the tests**

```bash
dotnet test tests/ActualiteEtAbonnement.Tests --filter "ServiceNotificationDbTests" -v minimal
```

Expected: 2 tests PASS.

- [ ] **Step 8: Build full module**

```bash
dotnet build src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj -v minimal 2>&1 | tail -5
```

Expected: `Build succeeded.`

- [ ] **Step 9: Commit**

```bash
git add src/Modules/ActualiteEtAbonnement/Infrastructure/ src/Modules/ActualiteEtAbonnement/Loader/ tests/ActualiteEtAbonnement.Tests/Application/
git commit -m "feat(abonnement): add NotificationRepository, ServiceNotificationDb, update ModuleLoader"
```

---

## Task 3 — Application: ObtenirAbonnements + SupprimerAbonnement

**Files:**
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirAbonnements/ObtenirAbonnementsQuery.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirAbonnements/ObtenirAbonnementsQueryHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/SupprimerAbonnement/SupprimerAbonnementCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/SupprimerAbonnement/SupprimerAbonnementCommandHandler.cs`
- Create: `tests/ActualiteEtAbonnement.Tests/Application/SupprimerAbonnementTests.cs`

- [ ] **Step 1: Write the failing test**

Create `tests/ActualiteEtAbonnement.Tests/Application/SupprimerAbonnementTests.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using FluentAssertions;

namespace ActualiteEtAbonnement.Tests.Application;

public class SupprimerAbonnementTests
{
    private sealed class FakeAbonnementRepository : IAbonnementRepository
    {
        private readonly List<Abonnement> _store = [];

        public Task AjouterAsync(Abonnement abonnement, CancellationToken cancellationToken = default)
        {
            _store.Add(abonnement);
            return Task.CompletedTask;
        }

        public Task<bool> ExisteDejaAsync(Guid utilisateurId, string domaineMetier, CancellationToken cancellationToken = default)
            => Task.FromResult(_store.Any(a => a.UtilisateurId == utilisateurId && a.DomaineMetier == domaineMetier));

        public Task<IReadOnlyList<Abonnement>> ObtenirParDomaineAsync(string domaineMetier, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Abonnement>>(_store.Where(a => a.DomaineMetier == domaineMetier).ToList());

        public Task<IReadOnlyList<Abonnement>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Abonnement>>(_store.Where(a => a.UtilisateurId == utilisateurId).ToList());

        public Task SupprimerAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
        {
            var abonnement = _store.FirstOrDefault(a => a.Id == id && a.UtilisateurId == utilisateurId)
                ?? throw new AbonnementNonTrouveException(id);
            _store.Remove(abonnement);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Supprimer_QuandAbonnementExisteEtAppartientAuUser_Supprime()
    {
        var repo = new FakeAbonnementRepository();
        var userId = Guid.NewGuid();
        var abonnement = Abonnement.Creer(userId, "cloud", CanalDiffusion.InApp);
        await repo.AjouterAsync(abonnement);

        var handler = new SupprimerAbonnementCommandHandler(repo);
        await handler.Handle(new SupprimerAbonnementCommand(abonnement.Id, userId), default);

        var restants = await repo.ObtenirParUtilisateurAsync(userId);
        restants.Should().BeEmpty();
    }

    [Fact]
    public async Task Supprimer_QuandAbonnementAppartientAAutreUser_LeveException()
    {
        var repo = new FakeAbonnementRepository();
        var userId = Guid.NewGuid();
        var autreUserId = Guid.NewGuid();
        var abonnement = Abonnement.Creer(userId, "cloud", CanalDiffusion.InApp);
        await repo.AjouterAsync(abonnement);

        var handler = new SupprimerAbonnementCommandHandler(repo);
        var act = () => handler.Handle(new SupprimerAbonnementCommand(abonnement.Id, autreUserId), default);

        await act.Should().ThrowAsync<AbonnementNonTrouveException>();
    }
}
```

- [ ] **Step 2: Run to verify it fails**

```bash
dotnet test tests/ActualiteEtAbonnement.Tests --filter "SupprimerAbonnementTests" -v minimal
```

Expected: compile error — `SupprimerAbonnementCommand` and `SupprimerAbonnementCommandHandler` don't exist yet.

- [ ] **Step 3: Create ObtenirAbonnements feature**

Create `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirAbonnements/ObtenirAbonnementsQuery.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirAbonnements;

public record ObtenirAbonnementsQuery(Guid UtilisateurId) : IRequest<IReadOnlyList<Abonnement>>;
```

Create `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirAbonnements/ObtenirAbonnementsQueryHandler.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirAbonnements;

public class ObtenirAbonnementsQueryHandler(IAbonnementRepository repository)
    : IRequestHandler<ObtenirAbonnementsQuery, IReadOnlyList<Abonnement>>
{
    public Task<IReadOnlyList<Abonnement>> Handle(ObtenirAbonnementsQuery request, CancellationToken cancellationToken)
        => repository.ObtenirParUtilisateurAsync(request.UtilisateurId, cancellationToken);
}
```

- [ ] **Step 4: Create SupprimerAbonnement feature**

Create `src/Modules/ActualiteEtAbonnement/Application/Features/SupprimerAbonnement/SupprimerAbonnementCommand.cs`:

```csharp
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;

public record SupprimerAbonnementCommand(Guid AbonnementId, Guid UtilisateurId) : IRequest;
```

Create `src/Modules/ActualiteEtAbonnement/Application/Features/SupprimerAbonnement/SupprimerAbonnementCommandHandler.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;

public class SupprimerAbonnementCommandHandler(IAbonnementRepository repository)
    : IRequestHandler<SupprimerAbonnementCommand>
{
    public Task Handle(SupprimerAbonnementCommand request, CancellationToken cancellationToken)
        => repository.SupprimerAsync(request.AbonnementId, request.UtilisateurId, cancellationToken);
}
```

- [ ] **Step 5: Run the tests**

```bash
dotnet test tests/ActualiteEtAbonnement.Tests --filter "SupprimerAbonnementTests" -v minimal
```

Expected: 2 tests PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirAbonnements/ src/Modules/ActualiteEtAbonnement/Application/Features/SupprimerAbonnement/ tests/ActualiteEtAbonnement.Tests/Application/SupprimerAbonnementTests.cs
git commit -m "feat(abonnement): add ObtenirAbonnements query and SupprimerAbonnement command"
```

---

## Task 4 — Application: ObtenirNotifications + MarquerNotificationLue

**Files:**
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirNotifications/ObtenirNotificationsQuery.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirNotifications/ObtenirNotificationsQueryHandler.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/MarquerNotificationLue/MarquerNotificationLueCommand.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Application/Features/MarquerNotificationLue/MarquerNotificationLueCommandHandler.cs`

- [ ] **Step 1: Create ObtenirNotifications feature**

Create `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirNotifications/ObtenirNotificationsQuery.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirNotifications;

public record ObtenirNotificationsQuery(Guid UtilisateurId) : IRequest<IReadOnlyList<Notification>>;
```

Create `src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirNotifications/ObtenirNotificationsQueryHandler.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirNotifications;

public class ObtenirNotificationsQueryHandler(INotificationRepository repository)
    : IRequestHandler<ObtenirNotificationsQuery, IReadOnlyList<Notification>>
{
    public Task<IReadOnlyList<Notification>> Handle(ObtenirNotificationsQuery request, CancellationToken cancellationToken)
        => repository.ObtenirParUtilisateurAsync(request.UtilisateurId, cancellationToken);
}
```

- [ ] **Step 2: Create MarquerNotificationLue feature**

Create `src/Modules/ActualiteEtAbonnement/Application/Features/MarquerNotificationLue/MarquerNotificationLueCommand.cs`:

```csharp
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.MarquerNotificationLue;

public record MarquerNotificationLueCommand(Guid NotificationId, Guid UtilisateurId) : IRequest;
```

Create `src/Modules/ActualiteEtAbonnement/Application/Features/MarquerNotificationLue/MarquerNotificationLueCommandHandler.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.MarquerNotificationLue;

public class MarquerNotificationLueCommandHandler(INotificationRepository repository)
    : IRequestHandler<MarquerNotificationLueCommand>
{
    public Task Handle(MarquerNotificationLueCommand request, CancellationToken cancellationToken)
        => repository.MarquerCommeLueAsync(request.NotificationId, request.UtilisateurId, cancellationToken);
}
```

- [ ] **Step 3: Build to verify**

```bash
dotnet build src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj -v minimal 2>&1 | tail -5
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add src/Modules/ActualiteEtAbonnement/Application/Features/ObtenirNotifications/ src/Modules/ActualiteEtAbonnement/Application/Features/MarquerNotificationLue/
git commit -m "feat(abonnement): add ObtenirNotifications query and MarquerNotificationLue command"
```

---

## Task 5 — API: DTOs + Controllers

**Files:**
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/AbonnementDto.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/DTOs/NotificationDto.cs`
- Modify: `src/Modules/ActualiteEtAbonnement/Client/Controllers/AbonnementController.cs`
- Create: `src/Modules/ActualiteEtAbonnement/Client/Controllers/NotificationsController.cs`

- [ ] **Step 1: Create DTOs**

Create `src/Modules/ActualiteEtAbonnement/Client/DTOs/AbonnementDto.cs`:

```csharp
namespace CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;

public record AbonnementDto(Guid Id, string DomaineMetier, string Canal, DateTime DateAbonnement);
```

Create `src/Modules/ActualiteEtAbonnement/Client/DTOs/NotificationDto.cs`:

```csharp
namespace CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;

public record NotificationDto(Guid Id, string Titre, string Corps, string Canal, bool EstLue, DateTime DateEnvoi);
```

- [ ] **Step 2: Update `AbonnementController` with GET and DELETE**

Replace the full contents of `src/Modules/ActualiteEtAbonnement/Client/Controllers/AbonnementController.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirAbonnements;
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/abonnements")]
public class AbonnementController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var abonnements = await sender.Send(new ObtenirAbonnementsQuery(uid), ct);
        return Ok(abonnements.Select(a => new AbonnementDto(a.Id, a.DomaineMetier, a.Canal.ToString(), a.DateAbonnement)));
    }

    [HttpPost]
    public async Task<IActionResult> Abonner([FromBody] AbonnerDomaineRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new AbonnerDomaineCommand(uid, dto.DomaineMetier, dto.Canal), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Supprimer(Guid id, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new SupprimerAbonnementCommand(id, uid), ct);
        return NoContent();
    }
}
```

- [ ] **Step 3: Create `NotificationsController`**

Create `src/Modules/ActualiteEtAbonnement/Client/Controllers/NotificationsController.cs`:

```csharp
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.MarquerNotificationLue;
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirNotifications;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notifs = await sender.Send(new ObtenirNotificationsQuery(uid), ct);
        return Ok(notifs.Select(n => new NotificationDto(n.Id, n.Titre, n.Corps, n.Canal.ToString(), n.EstLue, n.DateEnvoi)));
    }

    [HttpPatch("{id:guid}/lue")]
    public async Task<IActionResult> MarquerLue(Guid id, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new MarquerNotificationLueCommand(id, uid), ct);
        return NoContent();
    }
}
```

- [ ] **Step 4: Build to verify**

```bash
dotnet build src/Modules/ActualiteEtAbonnement/ActualiteEtAbonnement.csproj -v minimal 2>&1 | tail -5
```

Expected: `Build succeeded.`

- [ ] **Step 5: Run all tests**

```bash
dotnet test tests/ActualiteEtAbonnement.Tests -v minimal
```

Expected: all tests PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Modules/ActualiteEtAbonnement/Client/
git commit -m "feat(abonnement): add DTOs, GET/DELETE abonnements endpoints, notifications controller"
```

---

## Task 6 — Frontend: update abonnements page

**Files:**
- Modify: `frontend/app/candidat/abonnements/page.tsx`

- [ ] **Step 1: Update the page**

Replace the full contents of `frontend/app/candidat/abonnements/page.tsx`:

```tsx
'use client'

import { useState, useEffect, useCallback } from 'react'
import { apiFetch } from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'

interface Abonnement {
  id: string
  domaineMetier: string
  canal: string
  dateAbonnement: string
}

export default function AbonnementsPage() {
  const [abonnements, setAbonnements] = useState<Abonnement[]>([])
  const [canal, setCanal] = useState<'Email' | 'InApp'>('Email')
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState('')

  const chargerAbonnements = useCallback(async () => {
    try {
      const data = await apiFetch('/api/abonnements')
      setAbonnements(data)
    } catch {
      // silently ignore list errors on mount
    }
  }, [])

  useEffect(() => {
    chargerAbonnements()
  }, [chargerAbonnements])

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setSuccess(false)
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch('/api/abonnements', {
        method: 'POST',
        body: JSON.stringify({
          domaineMetier: fd.get('domaine'),
          canal,
        }),
      })
      setSuccess(true)
      ;(e.target as HTMLFormElement).reset()
      await chargerAbonnements()
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erreur lors de l'abonnement")
    } finally {
      setLoading(false)
    }
  }

  async function handleSupprimer(id: string) {
    try {
      await apiFetch(`/api/abonnements/${id}`, { method: 'DELETE' })
      setAbonnements(prev => prev.filter(a => a.id !== id))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la suppression')
    }
  }

  return (
    <div className="flex flex-col gap-6 max-w-lg">
      <h1 className="text-2xl font-bold">Mes abonnements</h1>

      {abonnements.length > 0 && (
        <Card>
          <CardHeader><CardTitle>Abonnements actifs</CardTitle></CardHeader>
          <CardContent className="flex flex-col gap-2">
            {abonnements.map(a => (
              <div key={a.id} className="flex items-center justify-between border rounded p-2">
                <div>
                  <span className="font-medium">{a.domaineMetier}</span>
                  <span className="ml-2 text-xs text-muted-foreground">{a.canal}</span>
                </div>
                <Button variant="destructive" size="sm" onClick={() => handleSupprimer(a.id)}>
                  Supprimer
                </Button>
              </div>
            ))}
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader><CardTitle>S&apos;abonner à un domaine</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            {success && <Alert><AlertDescription>Abonnement créé avec succès !</AlertDescription></Alert>}
            {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="domaine">Domaine métier</Label>
              <Input
                id="domaine"
                name="domaine"
                placeholder="ex: Cloud Azure, DevOps, Data Science…"
                required
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label>Canal de notification</Label>
              <Select value={canal} onValueChange={(v) => setCanal(v as 'Email' | 'InApp')}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Email">Email</SelectItem>
                  <SelectItem value="InApp">In-App</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <Button type="submit" disabled={loading}>
              {loading ? 'Abonnement…' : "S'abonner"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
```

- [ ] **Step 2: Build the frontend**

```bash
cd frontend && npm run build 2>&1 | tail -15
```

Expected: build completes without TypeScript errors.

- [ ] **Step 3: Commit**

```bash
git add frontend/app/candidat/abonnements/page.tsx
git commit -m "feat(abonnement): update abonnements page with subscription list and delete"
```

---

## Verification — End-to-End Test Flow

After all tasks are complete, verify the full flow:

1. Start the API: `dotnet run --project src/Api` (or however the project is launched)
2. Log in as a candidat user
3. Navigate to **Mes abonnements** → form visible, list empty
4. Subscribe to `"Cloud Azure"` with canal `InApp` → list shows the new subscription
5. Via Swagger (`/swagger`) — `POST /api/annonces` (or the CatalogueEmploi endpoint) to publish an annonce in domain `"Cloud Azure"`
6. `GET /api/notifications` in Swagger → should return 1 notification with `estLue: false`
7. `PATCH /api/notifications/{id}/lue` → returns 204
8. `GET /api/notifications` again → `estLue: true`
9. Click **Supprimer** on the subscription in the UI → list becomes empty
10. Run all backend tests: `dotnet test tests/ActualiteEtAbonnement.Tests -v minimal` → all PASS

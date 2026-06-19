# Console Admin Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a functional admin console with article publication and content moderation (annonces + appels d'offre).

**Architecture:** Backend adds `SupprimerAnnonceCommand` and `SupprimerAppelOffreCommand` following the existing Vertical Slice + `IVerificateurPermission` pattern. Frontend adds 3 new pages under `/admin/` and updates the sidebar.

**Tech Stack:** .NET 10, MediatR, EF Core, xUnit, FluentAssertions, Next.js (App Router), TypeScript, shadcn/ui

## Global Constraints

- Handler first line MUST call `IVerificateurPermission.AutoriserAsync` — throw `PermissionRefuseeException` (never `UnauthorizedAccessException`) on failure
- Domain naming: French (entités, méthodes métier). Technical naming: English (Handler, Repository, Controller)
- `PermissionRefuseeException` lives in `CVTech.Modules.GestionIdentite.Contracts`
- `ActionSecurisee.ModererContenu` is the permission for suppression actions (already in enum)
- Frontend uses `apiFetch` from `@/lib/api` which injects JWT automatically
- Tests use `FakeVerificateurPermission(autorise: bool)` pattern — see existing fakes in `tests/CatalogueEmploi.Tests/Fakes/`

---

### Task 1: Backend — SupprimerAnnonce (CatalogueEmploi)

**Files:**
- Modify: `src/Modules/CatalogueEmploi/Domain/Interfaces/IAnnonceRepository.cs`
- Modify: `src/Modules/CatalogueEmploi/Infrastructure/Repositories/AnnonceRepository.cs`
- Modify: `tests/CatalogueEmploi.Tests/Fakes/FakeAnnonceRepository.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/SupprimerAnnonce/SupprimerAnnonceCommand.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/SupprimerAnnonce/SupprimerAnnonceCommandHandler.cs`
- Modify: `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`
- Modify: `tests/CatalogueEmploi.Tests/Application/PermissionHandlerTests.cs`

**Interfaces:**
- Produces: `DELETE /api/annonces/{id}` → 204 No Content (admin only)
- Produces: `SupprimerAnnonceCommand(UtilisateurId: Guid, AnnonceId: Guid)` : IRequest

- [ ] **Step 1: Write the failing permission test**

Add to `tests/CatalogueEmploi.Tests/Application/PermissionHandlerTests.cs`:

```csharp
[Fact]
public async Task UnNonAdminNePeutPasSupprimerUneAnnonce()
{
    var repo = new FakeAnnonceRepository();
    var handler = new SupprimerAnnonceCommandHandler(
        repo,
        new FakeVerificateurPermission(autorise: false)
    );
    var command = new SupprimerAnnonceCommand(Guid.NewGuid(), Guid.NewGuid());

    var act = () => handler.Handle(command, CancellationToken.None);

    await act.Should().ThrowAsync<PermissionRefuseeException>();
}
```

- [ ] **Step 2: Run test to verify it fails**

```
dotnet test tests/CatalogueEmploi.Tests/ --filter "UnNonAdminNePeutPasSupprimerUneAnnonce" -v
```
Expected: FAIL — `SupprimerAnnonceCommandHandler` not found.

- [ ] **Step 3: Add `SupprimerAsync` to `IAnnonceRepository`**

In `src/Modules/CatalogueEmploi/Domain/Interfaces/IAnnonceRepository.cs`, add:

```csharp
Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default);
```

- [ ] **Step 4: Implement `SupprimerAsync` in `AnnonceRepository`**

In `src/Modules/CatalogueEmploi/Infrastructure/Repositories/AnnonceRepository.cs`, add:

```csharp
public async Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default)
{
    var annonce = await db.Annonces.FindAsync([id], cancellationToken);
    if (annonce is null) return;
    annonce.Desactiver();
    await db.SaveChangesAsync(cancellationToken);
}
```

- [ ] **Step 5: Implement `SupprimerAsync` in `FakeAnnonceRepository`**

In `tests/CatalogueEmploi.Tests/Fakes/FakeAnnonceRepository.cs`, add:

```csharp
public Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default)
{
    var annonce = _annonces.FirstOrDefault(a => a.Id == id);
    annonce?.Desactiver();
    return Task.CompletedTask;
}
```

- [ ] **Step 6: Create `SupprimerAnnonceCommand`**

Create `src/Modules/CatalogueEmploi/Application/Features/SupprimerAnnonce/SupprimerAnnonceCommand.cs`:

```csharp
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.SupprimerAnnonce;

public record SupprimerAnnonceCommand(
    Guid UtilisateurId,
    Guid AnnonceId
) : IRequest;
```

- [ ] **Step 7: Create `SupprimerAnnonceCommandHandler`**

Create `src/Modules/CatalogueEmploi/Application/Features/SupprimerAnnonce/SupprimerAnnonceCommandHandler.cs`:

```csharp
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.SupprimerAnnonce;

public class SupprimerAnnonceCommandHandler(
    IAnnonceRepository annonceRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<SupprimerAnnonceCommand>
{
    public async Task Handle(SupprimerAnnonceCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.ModererContenu, cancellationToken))
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ModererContenu));

        await annonceRepository.SupprimerAsync(request.AnnonceId, cancellationToken);
    }
}
```

- [ ] **Step 8: Run test to verify it passes**

```
dotnet test tests/CatalogueEmploi.Tests/ --filter "UnNonAdminNePeutPasSupprimerUneAnnonce" -v
```
Expected: PASS

- [ ] **Step 9: Add DELETE endpoint to `AnnonceController`**

In `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`, add import and method:

```csharp
using CVTech.Modules.CatalogueEmploi.Application.Features.SupprimerAnnonce;
```

```csharp
[HttpDelete("{annonceId:guid}")]
public async Task<IActionResult> Supprimer(Guid annonceId, CancellationToken ct)
{
    var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
    await sender.Send(new SupprimerAnnonceCommand(utilisateurId, annonceId), ct);
    return NoContent();
}
```

- [ ] **Step 10: Build and verify no compile errors**

```
dotnet build src/Modules/CatalogueEmploi/
```
Expected: Build succeeded, 0 error(s).

- [ ] **Step 11: Commit**

```bash
git add src/Modules/CatalogueEmploi/ tests/CatalogueEmploi.Tests/
git commit -m "feat(catalogue-emploi): add SupprimerAnnonce command and DELETE /api/annonces/{id}"
```

---

### Task 2: Backend — SupprimerAppelOffre (AppelOffreFreelance)

**Files:**
- Modify: `src/Modules/AppelOffreFreelance/Domain/Interfaces/IAppelOffreRepository.cs`
- Modify: `src/Modules/AppelOffreFreelance/Infrastructure/Repositories/AppelOffreRepository.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/SupprimerAppelOffre/SupprimerAppelOffreCommand.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/SupprimerAppelOffre/SupprimerAppelOffreCommandHandler.cs`
- Modify: `src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`
- Create/Modify: `tests/AppelOffreFreelance.Tests/Application/PermissionHandlerTests.cs`

**Interfaces:**
- Consumes: `ActionSecurisee.ModererContenu` (same as Task 1)
- Produces: `DELETE /api/appels-offre/{id}` → 204 No Content (admin only)
- Produces: `SupprimerAppelOffreCommand(UtilisateurId: Guid, AppelOffreId: Guid)` : IRequest

- [ ] **Step 1: Check test project structure**

```
find tests/AppelOffreFreelance.Tests -type f -name "*.cs" | sort
```

If no `Fakes/` directory exists, create the fake files needed.

- [ ] **Step 2: Create `FakeVerificateurPermission` if missing**

Create `tests/AppelOffreFreelance.Tests/Fakes/FakeVerificateurPermission.cs`:

```csharp
using CVTech.Modules.GestionIdentite.Contracts;

namespace AppelOffreFreelance.Tests.Fakes;

public class FakeVerificateurPermission(bool autorise) : IVerificateurPermission
{
    public Task<bool> AutoriserAsync(Guid utilisateurId, ActionSecurisee action, CancellationToken cancellationToken = default)
        => Task.FromResult(autorise);
}
```

- [ ] **Step 3: Create `FakeAppelOffreRepository` if missing**

Create `tests/AppelOffreFreelance.Tests/Fakes/FakeAppelOffreRepository.cs`:

```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;

namespace AppelOffreFreelance.Tests.Fakes;

public class FakeAppelOffreRepository : IAppelOffreRepository
{
    private readonly List<AppelOffre> _items = [];

    public void Ajouter(AppelOffre ao) => _items.Add(ao);

    public Task<AppelOffre?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_items.FirstOrDefault(a => a.Id == id));

    public Task AjouterAsync(AppelOffre appelOffre, CancellationToken cancellationToken = default)
    {
        _items.Add(appelOffre);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AppelOffre>> ListerOuvertsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AppelOffre>>(_items.Where(a => a.Statut == StatutAppelOffre.Ouvert).ToList());

    public Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _items.FirstOrDefault(a => a.Id == id)?.Clore();
        return Task.CompletedTask;
    }
}
```

- [ ] **Step 4: Write the failing permission test**

Create `tests/AppelOffreFreelance.Tests/Application/PermissionHandlerTests.cs`:

```csharp
using AppelOffreFreelance.Tests.Fakes;
using CVTech.Modules.AppelOffreFreelance.Application.Features.SupprimerAppelOffre;
using CVTech.Modules.GestionIdentite.Contracts;
using FluentAssertions;

namespace AppelOffreFreelance.Tests.Application;

public class PermissionHandlerTests
{
    [Fact]
    public async Task UnNonAdminNePeutPasSupprimerUnAppelOffre()
    {
        var handler = new SupprimerAppelOffreCommandHandler(
            new FakeAppelOffreRepository(),
            new FakeVerificateurPermission(autorise: false)
        );
        var command = new SupprimerAppelOffreCommand(Guid.NewGuid(), Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }
}
```

- [ ] **Step 5: Run test to verify it fails**

```
dotnet test tests/AppelOffreFreelance.Tests/ --filter "UnNonAdminNePeutPasSupprimerUnAppelOffre" -v
```
Expected: FAIL — `SupprimerAppelOffreCommandHandler` not found.

- [ ] **Step 6: Add `SupprimerAsync` to `IAppelOffreRepository`**

In `src/Modules/AppelOffreFreelance/Domain/Interfaces/IAppelOffreRepository.cs`, add:

```csharp
Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default);
```

- [ ] **Step 7: Implement `SupprimerAsync` in `AppelOffreRepository`**

In `src/Modules/AppelOffreFreelance/Infrastructure/Repositories/AppelOffreRepository.cs`, add:

```csharp
public async Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default)
{
    var ao = await db.AppelsOffre.FindAsync([id], cancellationToken);
    if (ao is null) return;
    ao.Clore();
    await db.SaveChangesAsync(cancellationToken);
}
```

- [ ] **Step 8: Create `SupprimerAppelOffreCommand`**

Create `src/Modules/AppelOffreFreelance/Application/Features/SupprimerAppelOffre/SupprimerAppelOffreCommand.cs`:

```csharp
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SupprimerAppelOffre;

public record SupprimerAppelOffreCommand(
    Guid UtilisateurId,
    Guid AppelOffreId
) : IRequest;
```

- [ ] **Step 9: Create `SupprimerAppelOffreCommandHandler`**

Create `src/Modules/AppelOffreFreelance/Application/Features/SupprimerAppelOffre/SupprimerAppelOffreCommandHandler.cs`:

```csharp
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SupprimerAppelOffre;

public class SupprimerAppelOffreCommandHandler(
    IAppelOffreRepository repository,
    IVerificateurPermission verificateur
) : IRequestHandler<SupprimerAppelOffreCommand>
{
    public async Task Handle(SupprimerAppelOffreCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.ModererContenu, cancellationToken))
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ModererContenu));

        await repository.SupprimerAsync(request.AppelOffreId, cancellationToken);
    }
}
```

- [ ] **Step 10: Run test to verify it passes**

```
dotnet test tests/AppelOffreFreelance.Tests/ --filter "UnNonAdminNePeutPasSupprimerUnAppelOffre" -v
```
Expected: PASS

- [ ] **Step 11: Add DELETE endpoint to `AppelOffreController`**

In `src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`, add import and method:

```csharp
using CVTech.Modules.AppelOffreFreelance.Application.Features.SupprimerAppelOffre;
```

```csharp
[HttpDelete("{appelOffreId:guid}")]
public async Task<IActionResult> Supprimer(Guid appelOffreId, CancellationToken ct)
{
    var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
    await sender.Send(new SupprimerAppelOffreCommand(uid, appelOffreId), ct);
    return NoContent();
}
```

- [ ] **Step 12: Build and verify**

```
dotnet build src/Modules/AppelOffreFreelance/
```
Expected: Build succeeded, 0 error(s).

- [ ] **Step 13: Commit**

```bash
git add src/Modules/AppelOffreFreelance/ tests/AppelOffreFreelance.Tests/
git commit -m "feat(appel-offre): add SupprimerAppelOffre command and DELETE /api/appels-offre/{id}"
```

---

### Task 3: Frontend — Sidebar admin + page publier article

**Files:**
- Modify: `frontend/app/admin/layout.tsx`
- Create: `frontend/app/admin/actualites/publier/page.tsx`

**Interfaces:**
- Consumes: `POST /api/actualites` with body `{ titre: string, contenu: string, domaineMetier: string | null }`
- Consumes: `apiFetch` from `@/lib/api`

- [ ] **Step 1: Update sidebar in `frontend/app/admin/layout.tsx`**

Replace the entire file content:

```tsx
'use client'

import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { logout } from '@/lib/auth'
import { Button } from '@/components/ui/button'

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter()

  function handleLogout() {
    logout()
    router.replace('/connexion')
  }

  return (
    <div className="min-h-screen flex">
      <aside className="w-56 border-r bg-muted/30 flex flex-col p-4 gap-1">
        <Link href="/" className="font-bold text-lg mb-4">CVTech Admin</Link>
        <Link href="/admin/dashboard" className="text-sm hover:underline py-1">Tableau de bord</Link>
        <p className="text-xs text-muted-foreground mt-3 mb-1 uppercase tracking-wide">Contenu</p>
        <Link href="/admin/actualites/publier" className="text-sm hover:underline py-1">Publier un article</Link>
        <p className="text-xs text-muted-foreground mt-3 mb-1 uppercase tracking-wide">Modération</p>
        <Link href="/admin/annonces" className="text-sm hover:underline py-1">Annonces d'emploi</Link>
        <Link href="/admin/appels-offre" className="text-sm hover:underline py-1">Appels d'offre</Link>
        <div className="mt-auto">
          <Button variant="ghost" size="sm" className="w-full justify-start" onClick={handleLogout}>
            Déconnexion
          </Button>
        </div>
      </aside>
      <main className="flex-1 p-8">{children}</main>
    </div>
  )
}
```

- [ ] **Step 2: Create `frontend/app/admin/actualites/publier/page.tsx`**

```tsx
'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { apiFetch } from '@/lib/api'
import { Card, CardContent } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { Alert, AlertDescription } from '@/components/ui/alert'

export default function PublierArticlePage() {
  const router = useRouter()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch('/api/actualites', {
        method: 'POST',
        body: JSON.stringify({
          titre: fd.get('titre'),
          contenu: fd.get('contenu'),
          domaineMetier: fd.get('domaineMetier') || null,
        }),
      })
      setSuccess(true)
      ;(e.target as HTMLFormElement).reset()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la publication')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-lg">
      <h1 className="text-2xl font-bold mb-6">Publier un article</h1>
      <Card>
        <CardContent className="pt-6">
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
            {success && <Alert><AlertDescription>Article publié avec succès.</AlertDescription></Alert>}
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="titre">Titre</Label>
              <Input id="titre" name="titre" placeholder="ex: Tendances DevOps 2026" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="contenu">Contenu</Label>
              <Textarea id="contenu" name="contenu" rows={8} required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="domaineMetier">Domaine métier (optionnel)</Label>
              <Input id="domaineMetier" name="domaineMetier" placeholder="ex: DevOps, Cloud Azure…" />
            </div>
            <Button type="submit" disabled={loading}>
              {loading ? 'Publication…' : 'Publier l\'article'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
```

- [ ] **Step 3: Test in browser**

Navigate to `/admin/actualites/publier`, fill the form, submit. Verify 201 response in network tab and success message appears.

- [ ] **Step 4: Commit**

```bash
git add frontend/app/admin/
git commit -m "feat(admin): add article publication page and update sidebar"
```

---

### Task 4: Frontend — Page modération annonces

**Files:**
- Create: `frontend/app/admin/annonces/page.tsx`

**Interfaces:**
- Consumes: `GET /api/annonces` → `{ id, titre, domaineMetier, typeContrat, datePublication, entrepriseId }[]`
- Consumes: `DELETE /api/annonces/{id}` → 204

- [ ] **Step 1: Create `frontend/app/admin/annonces/page.tsx`**

```tsx
'use client'

import { useEffect, useState } from 'react'
import { apiFetch } from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription } from '@/components/ui/alert'

interface Annonce {
  id: string
  titre: string
  domaineMetier: string
  typeContrat: string
  datePublication: string
  entrepriseId: string
}

export default function ModerationAnnoncesPage() {
  const [annonces, setAnnonces] = useState<Annonce[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    apiFetch<Annonce[]>('/api/annonces')
      .then(setAnnonces)
      .catch(err => setError(err instanceof Error ? err.message : 'Erreur'))
      .finally(() => setLoading(false))
  }, [])

  async function handleSupprimer(id: string) {
    try {
      await apiFetch(`/api/annonces/${id}`, { method: 'DELETE' })
      setAnnonces(prev => prev.filter(a => a.id !== id))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la suppression')
    }
  }

  if (loading) return <p>Chargement…</p>

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold">Modération — Annonces d'emploi</h1>
      {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
      {!error && annonces.length === 0 && (
        <p className="text-muted-foreground">Aucune annonce active.</p>
      )}
      {annonces.map(a => (
        <Card key={a.id}>
          <CardHeader className="pb-2">
            <div className="flex items-start justify-between gap-4">
              <CardTitle className="text-base">{a.titre}</CardTitle>
              <Button
                variant="destructive"
                size="sm"
                onClick={() => handleSupprimer(a.id)}
              >
                Supprimer
              </Button>
            </div>
          </CardHeader>
          <CardContent className="text-sm text-muted-foreground flex gap-2 flex-wrap">
            <Badge variant="outline">{a.typeContrat}</Badge>
            <span>{a.domaineMetier}</span>
            <span>·</span>
            <span>{new Date(a.datePublication).toLocaleDateString('fr-FR')}</span>
          </CardContent>
        </Card>
      ))}
    </div>
  )
}
```

- [ ] **Step 2: Test in browser**

Navigate to `/admin/annonces`. Verify list loads. Click "Supprimer" on one item — verify it disappears from the list and returns 204 in network tab.

- [ ] **Step 3: Commit**

```bash
git add frontend/app/admin/annonces/
git commit -m "feat(admin): add annonces moderation page"
```

---

### Task 5: Frontend — Page modération appels d'offre

**Files:**
- Create: `frontend/app/admin/appels-offre/page.tsx`

**Interfaces:**
- Consumes: `GET /api/appels-offre` → `{ id, titre, domaineMetier, budgetMax, deadline, datePublication, statut }[]`
- Consumes: `DELETE /api/appels-offre/{id}` → 204

- [ ] **Step 1: Create `frontend/app/admin/appels-offre/page.tsx`**

```tsx
'use client'

import { useEffect, useState } from 'react'
import { apiFetch } from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription } from '@/components/ui/alert'

interface AppelOffre {
  id: string
  titre: string
  domaineMetier: string
  budgetMax: number
  deadline: string
  datePublication: string
  statut: string
}

export default function ModerationAppelsOffrePage() {
  const [appels, setAppels] = useState<AppelOffre[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    apiFetch<AppelOffre[]>('/api/appels-offre')
      .then(setAppels)
      .catch(err => setError(err instanceof Error ? err.message : 'Erreur'))
      .finally(() => setLoading(false))
  }, [])

  async function handleSupprimer(id: string) {
    try {
      await apiFetch(`/api/appels-offre/${id}`, { method: 'DELETE' })
      setAppels(prev => prev.filter(a => a.id !== id))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la suppression')
    }
  }

  if (loading) return <p>Chargement…</p>

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold">Modération — Appels d'offre freelance</h1>
      {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
      {!error && appels.length === 0 && (
        <p className="text-muted-foreground">Aucun appel d'offre actif.</p>
      )}
      {appels.map(a => (
        <Card key={a.id}>
          <CardHeader className="pb-2">
            <div className="flex items-start justify-between gap-4">
              <CardTitle className="text-base">{a.titre}</CardTitle>
              <Button
                variant="destructive"
                size="sm"
                onClick={() => handleSupprimer(a.id)}
              >
                Supprimer
              </Button>
            </div>
          </CardHeader>
          <CardContent className="text-sm text-muted-foreground flex gap-2 flex-wrap">
            <Badge variant="outline">{a.statut}</Badge>
            <span>{a.domaineMetier}</span>
            <span>·</span>
            <span>Budget max : {a.budgetMax.toLocaleString('fr-FR')} €</span>
            <span>·</span>
            <span>Deadline : {new Date(a.deadline).toLocaleDateString('fr-FR')}</span>
          </CardContent>
        </Card>
      ))}
    </div>
  )
}
```

- [ ] **Step 2: Test in browser**

Navigate to `/admin/appels-offre`. Verify list loads. Click "Supprimer" — verify 204 and item disappears.

- [ ] **Step 3: Run all tests**

```
dotnet test
```
Expected: all tests pass.

- [ ] **Step 4: Commit**

```bash
git add frontend/app/admin/appels-offre/
git commit -m "feat(admin): add appels-offre moderation page"
```

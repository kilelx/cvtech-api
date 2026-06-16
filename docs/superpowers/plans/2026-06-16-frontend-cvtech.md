# Frontend CVTech Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a Next.js 14 App Router frontend for CVTech with 3 functional user journeys (Candidat, Entreprise, Admin) connected to the .NET 10 API.

**Architecture:** Route groups per role `(public)/(auth)/(candidat)/(entreprise)/(admin)`, each with its own layout. JWT stored in localStorage + non-httpOnly cookie. Middleware protects role-specific routes. No server components — all pages are `'use client'` for simplicity.

**Tech Stack:** Next.js 14 App Router, TypeScript, Tailwind CSS, shadcn/ui, native fetch

---

## File Map

### API changes (backend)
- Create: `src/Modules/CatalogueEmploi/Application/Features/ListerAnnonces/ListerAnnoncesQuery.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/ListerAnnonces/ListerAnnoncesQueryHandler.cs`
- Modify: `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/ListerAppelsOffre/ListerAppelsOffreQuery.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/ListerAppelsOffre/ListerAppelsOffreQueryHandler.cs`
- Modify: `src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`

### Frontend
- `frontend/lib/auth.ts` — token get/set/clear, JWT payload decode, role extraction
- `frontend/lib/api.ts` — fetch wrapper with auto Authorization header + 401 redirect
- `frontend/middleware.ts` — cookie-based route guard by role
- `frontend/app/layout.tsx` — root layout (html/body)
- `frontend/app/page.tsx` — home: redirect based on role
- `frontend/app/(public)/layout.tsx` — navbar with login/register links
- `frontend/app/(public)/annonces/page.tsx` — public list + Postuler modal (Candidat only)
- `frontend/app/(public)/appels-offre/page.tsx` — public list + Soumettre modal (Candidat only)
- `frontend/app/(auth)/layout.tsx` — minimal centered layout
- `frontend/app/(auth)/connexion/page.tsx` — login form
- `frontend/app/(auth)/inscription/candidat/page.tsx` — candidat registration
- `frontend/app/(auth)/inscription/entreprise/page.tsx` — entreprise registration
- `frontend/app/(candidat)/layout.tsx` — sidebar nav for candidat
- `frontend/app/(candidat)/dashboard/page.tsx` — welcome + quick links
- `frontend/app/(candidat)/abonnements/page.tsx` — subscribe to domaine métier
- `frontend/app/(entreprise)/layout.tsx` — sidebar nav for entreprise
- `frontend/app/(entreprise)/dashboard/page.tsx` — welcome + quick links
- `frontend/app/(entreprise)/annonces/publier/page.tsx` — publish job listing form
- `frontend/app/(entreprise)/appels-offre/publier/page.tsx` — publish freelance RFP form
- `frontend/app/(admin)/layout.tsx` — admin layout
- `frontend/app/(admin)/dashboard/page.tsx` — "En construction" page

---

## Task 1: Add GET /api/annonces endpoint

**Files:**
- Create: `src/Modules/CatalogueEmploi/Application/Features/ListerAnnonces/ListerAnnoncesQuery.cs`
- Create: `src/Modules/CatalogueEmploi/Application/Features/ListerAnnonces/ListerAnnoncesQueryHandler.cs`
- Modify: `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`

- [ ] **Step 1: Create the query**

```csharp
// src/Modules/CatalogueEmploi/Application/Features/ListerAnnonces/ListerAnnoncesQuery.cs
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ListerAnnonces;

public record ListerAnnoncesQuery : IRequest<IReadOnlyList<AnnonceEmploi>>;
```

- [ ] **Step 2: Create the query handler**

```csharp
// src/Modules/CatalogueEmploi/Application/Features/ListerAnnonces/ListerAnnoncesQueryHandler.cs
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ListerAnnonces;

public class ListerAnnoncesQueryHandler(IAnnonceRepository repository)
    : IRequestHandler<ListerAnnoncesQuery, IReadOnlyList<AnnonceEmploi>>
{
    public Task<IReadOnlyList<AnnonceEmploi>> Handle(ListerAnnoncesQuery request, CancellationToken cancellationToken)
        => repository.ListerActivesAsync(cancellationToken);
}
```

- [ ] **Step 3: Add GET endpoint to AnnonceController**

Replace the full contents of `src/Modules/CatalogueEmploi/Client/Controllers/AnnonceController.cs`:

```csharp
using CVTech.Modules.CatalogueEmploi.Application.Features.ListerAnnonces;
using CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;
using CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;
using CVTech.Modules.CatalogueEmploi.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.CatalogueEmploi.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/annonces")]
public class AnnonceController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var annonces = await sender.Send(new ListerAnnoncesQuery(), ct);
        return Ok(annonces.Select(a => new
        {
            a.Id,
            a.Titre,
            a.Description,
            a.DomaineMetier,
            TypeContrat = a.TypeContrat.ToString(),
            a.EntrepriseId,
            a.DatePublication,
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierAnnonceCommand(utilisateurId, dto.Titre, dto.Description, dto.DomaineMetier, dto.TypeContrat), ct);
        return CreatedAtAction(nameof(Publier), new { id }, new { id });
    }

    [HttpPost("{annonceId:guid}/candidatures")]
    public async Task<IActionResult> Postuler(Guid annonceId, [FromBody] PostulerAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PostulerAnnonceCommand(utilisateurId, annonceId, dto.CurriculumVitaeId, dto.LettreMotivation), ct);
        return CreatedAtAction(nameof(Postuler), new { id }, new { id });
    }
}
```

- [ ] **Step 4: Verify compile**

```bash
dotnet build src/CVTech.Api/CVTech.Api.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 2: Add GET /api/appels-offre endpoint

**Files:**
- Create: `src/Modules/AppelOffreFreelance/Application/Features/ListerAppelsOffre/ListerAppelsOffreQuery.cs`
- Create: `src/Modules/AppelOffreFreelance/Application/Features/ListerAppelsOffre/ListerAppelsOffreQueryHandler.cs`
- Modify: `src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`

- [ ] **Step 1: Create the query**

```csharp
// src/Modules/AppelOffreFreelance/Application/Features/ListerAppelsOffre/ListerAppelsOffreQuery.cs
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.ListerAppelsOffre;

public record ListerAppelsOffreQuery : IRequest<IReadOnlyList<AppelOffre>>;
```

- [ ] **Step 2: Create the query handler**

```csharp
// src/Modules/AppelOffreFreelance/Application/Features/ListerAppelsOffre/ListerAppelsOffreQueryHandler.cs
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.ListerAppelsOffre;

public class ListerAppelsOffreQueryHandler(IAppelOffreRepository repository)
    : IRequestHandler<ListerAppelsOffreQuery, IReadOnlyList<AppelOffre>>
{
    public Task<IReadOnlyList<AppelOffre>> Handle(ListerAppelsOffreQuery request, CancellationToken cancellationToken)
        => repository.ListerOuvertsAsync(cancellationToken);
}
```

- [ ] **Step 3: Add GET endpoint to AppelOffreController**

Replace the full contents of `src/Modules/AppelOffreFreelance/Client/Controllers/AppelOffreController.cs`:

```csharp
using CVTech.Modules.AppelOffreFreelance.Application.Features.ListerAppelsOffre;
using CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;
using CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;
using CVTech.Modules.AppelOffreFreelance.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.AppelOffreFreelance.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/appels-offre")]
public class AppelOffreController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var appels = await sender.Send(new ListerAppelsOffreQuery(), ct);
        return Ok(appels.Select(a => new
        {
            a.Id,
            a.Titre,
            a.Contexte,
            a.DomaineMetier,
            a.BudgetMax,
            a.Deadline,
            a.EntrepriseId,
            a.DatePublication,
            Statut = a.Statut.ToString(),
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierAppelOffreRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierAppelOffreCommand(uid, dto.Titre, dto.Contexte, dto.DomaineMetier, dto.BudgetMax, dto.Deadline), ct);
        return CreatedAtAction(nameof(Publier), new { id }, new { id });
    }

    [HttpPost("{appelOffreId:guid}/propositions")]
    public async Task<IActionResult> SoumettreProposition(Guid appelOffreId, [FromBody] SoumettrePropositionRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new SoumettrePropositionCommand(uid, appelOffreId, dto.TauxJournalierMoyen, dto.DureeEstimeeJours, dto.Methodologie), ct);
        return CreatedAtAction(nameof(SoumettreProposition), new { id }, new { id });
    }
}
```

- [ ] **Step 4: Build and smoke-test**

```bash
dotnet build src/CVTech.Api/CVTech.Api.csproj
```

Expected: Build succeeded, 0 errors.

- [ ] **Step 5: Restart API and verify GET endpoints**

```bash
pkill -f "CVTech.Api" 2>/dev/null; sleep 1
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/CVTech.Api/CVTech.Api.csproj &
sleep 5
python3 -c "
import urllib.request, json
resp = urllib.request.urlopen('http://localhost:5000/api/annonces')
print('annonces:', resp.status, json.loads(resp.read().decode()))
resp2 = urllib.request.urlopen('http://localhost:5000/api/appels-offre')
print('appels-offre:', resp2.status, json.loads(resp2.read().decode()))
"
```

Expected: Both return `200` with JSON arrays (may be empty if no data, that's fine).

---

## Task 3: Scaffold Next.js + shadcn/ui

**Files:** creates `frontend/` directory with full Next.js project

- [ ] **Step 1: Create Next.js project**

Run from repo root (`/path/to/ia-skills`):

```bash
npx create-next-app@latest frontend \
  --typescript \
  --tailwind \
  --app \
  --no-src-dir \
  --import-alias "@/*" \
  --yes
```

Expected: `frontend/` created with `app/`, `package.json`, `tsconfig.json`, `tailwind.config.ts`.

- [ ] **Step 2: Init shadcn/ui**

```bash
cd frontend
npx shadcn@latest init --defaults
```

Expected: `components/ui/` created, `components.json` added.

- [ ] **Step 3: Add shadcn components**

```bash
cd frontend
npx shadcn@latest add button card input label badge dialog select alert textarea
```

Expected: Components added to `components/ui/`.

- [ ] **Step 4: Verify dev server starts**

```bash
cd frontend && npm run dev &
sleep 4
python3 -c "import urllib.request; r = urllib.request.urlopen('http://localhost:3000'); print('Status:', r.status)"
```

Expected: `Status: 200`. Kill the dev server after: `pkill -f "next dev"`.

---

## Task 4: Auth utilities + API wrapper

**Files:**
- Create: `frontend/lib/auth.ts`
- Create: `frontend/lib/api.ts`

- [ ] **Step 1: Create lib/auth.ts**

```typescript
// frontend/lib/auth.ts
const TOKEN_KEY = 'cvtech_token'

export type Role = 'Candidat' | 'Entreprise' | 'Administrateur'

export interface AuthPayload {
  utilisateurId: string
  role: Role
}

export function setToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token)
  document.cookie = `token=${token}; path=/; max-age=86400; SameSite=Lax`
}

export function getToken(): string | null {
  if (typeof window === 'undefined') return null
  return localStorage.getItem(TOKEN_KEY)
}

export function getAuth(): AuthPayload | null {
  const token = getToken()
  if (!token) return null
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const payload = JSON.parse(atob(base64))
    return {
      utilisateurId: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
      role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
    }
  } catch {
    return null
  }
}

export function logout(): void {
  localStorage.removeItem(TOKEN_KEY)
  document.cookie = 'token=; path=/; max-age=0'
}
```

- [ ] **Step 2: Create lib/api.ts**

```typescript
// frontend/lib/api.ts
import { getToken, logout } from './auth'

const API_BASE = 'http://localhost:5000'

export class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message)
  }
}

export async function apiFetch<T = unknown>(
  path: string,
  options?: RequestInit
): Promise<T> {
  const token = getToken()
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options?.headers as Record<string, string>),
  }
  if (token) headers['Authorization'] = `Bearer ${token}`

  const res = await fetch(`${API_BASE}${path}`, { ...options, headers })

  if (res.status === 401) {
    logout()
    window.location.href = '/connexion'
    throw new ApiError(401, 'Non autorisé')
  }

  if (res.status === 204) return undefined as T

  const body = await res.json().catch(() => ({}))

  if (!res.ok) {
    throw new ApiError(res.status, body.error ?? `Erreur ${res.status}`)
  }

  return body as T
}
```

- [ ] **Step 3: Verify TypeScript compiles**

```bash
cd frontend && npx tsc --noEmit
```

Expected: No errors.

---

## Task 5: Middleware (route protection by role)

**Files:**
- Create: `frontend/middleware.ts`

- [ ] **Step 1: Create middleware.ts**

```typescript
// frontend/middleware.ts
import { NextRequest, NextResponse } from 'next/server'

type Role = 'Candidat' | 'Entreprise' | 'Administrateur'

const PROTECTED: Array<{ prefix: string; role: Role }> = [
  { prefix: '/candidat', role: 'Candidat' },
  { prefix: '/entreprise', role: 'Entreprise' },
  { prefix: '/admin', role: 'Administrateur' },
]

function getRoleFromToken(token: string): Role | null {
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const payload = JSON.parse(atob(base64))
    return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null
  } catch {
    return null
  }
}

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl
  const token = request.cookies.get('token')?.value

  for (const { prefix, role } of PROTECTED) {
    if (pathname.startsWith(prefix)) {
      if (!token) {
        return NextResponse.redirect(
          new URL(`/connexion?redirect=${encodeURIComponent(pathname)}`, request.url)
        )
      }
      const userRole = getRoleFromToken(token)
      if (userRole !== role) {
        return NextResponse.redirect(new URL('/connexion', request.url))
      }
    }
  }

  return NextResponse.next()
}

export const config = {
  matcher: ['/candidat/:path*', '/entreprise/:path*', '/admin/:path*'],
}
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd frontend && npx tsc --noEmit
```

Expected: No errors.

---

## Task 6: Root layout + home page

**Files:**
- Modify: `frontend/app/layout.tsx`
- Modify: `frontend/app/page.tsx`
- Delete: `frontend/app/globals.css` content (keep Tailwind directives only)

- [ ] **Step 1: Update root layout**

Replace `frontend/app/layout.tsx`:

```tsx
// frontend/app/layout.tsx
import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import './globals.css'

const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'CVTech — Plateforme emploi & freelance tech',
  description: 'Job board et marketplace freelance pour les métiers de la tech',
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="fr">
      <body className={inter.className}>{children}</body>
    </html>
  )
}
```

- [ ] **Step 2: Create home page (redirect by role)**

Replace `frontend/app/page.tsx`:

```tsx
// frontend/app/page.tsx
'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { getAuth } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import Link from 'next/link'

export default function HomePage() {
  const router = useRouter()
  const auth = typeof window !== 'undefined' ? getAuth() : null

  useEffect(() => {
    if (!auth) return
    if (auth.role === 'Candidat') router.replace('/candidat/dashboard')
    else if (auth.role === 'Entreprise') router.replace('/entreprise/dashboard')
    else if (auth.role === 'Administrateur') router.replace('/admin/dashboard')
  }, [auth, router])

  return (
    <main className="min-h-screen flex flex-col items-center justify-center gap-6 p-8">
      <h1 className="text-4xl font-bold">CVTech</h1>
      <p className="text-muted-foreground text-lg text-center max-w-md">
        La plateforme unifiée pour trouver un emploi, décrocher une mission freelance et rester informé.
      </p>
      <div className="flex gap-4">
        <Button asChild><Link href="/annonces">Voir les annonces</Link></Button>
        <Button variant="outline" asChild><Link href="/connexion">Se connecter</Link></Button>
      </div>
    </main>
  )
}
```

---

## Task 7: Auth pages (connexion + inscription)

**Files:**
- Create: `frontend/app/(auth)/layout.tsx`
- Create: `frontend/app/(auth)/connexion/page.tsx`
- Create: `frontend/app/(auth)/inscription/candidat/page.tsx`
- Create: `frontend/app/(auth)/inscription/entreprise/page.tsx`

- [ ] **Step 1: Auth layout**

```tsx
// frontend/app/(auth)/layout.tsx
export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-muted/30 p-4">
      {children}
    </div>
  )
}
```

- [ ] **Step 2: Connexion page**

Create `frontend/app/(auth)/connexion/page.tsx`:

```tsx
'use client'

import { useState } from 'react'
import { useRouter, useSearchParams } from 'next/navigation'
import { apiFetch } from '@/lib/api'
import { setToken } from '@/lib/auth'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription } from '@/components/ui/alert'
import Link from 'next/link'

interface ConnexionResponse {
  utilisateurId: string
  role: string
  token: string
}

export default function ConnexionPage() {
  const router = useRouter()
  const params = useSearchParams()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      const res = await apiFetch<ConnexionResponse>('/api/identite/connexion', {
        method: 'POST',
        body: JSON.stringify({ email: fd.get('email'), motDePasse: fd.get('motDePasse') }),
      })
      setToken(res.token)
      const redirect = params.get('redirect')
      if (redirect) router.replace(redirect)
      else if (res.role === 'Candidat') router.replace('/candidat/dashboard')
      else if (res.role === 'Entreprise') router.replace('/entreprise/dashboard')
      else if (res.role === 'Administrateur') router.replace('/admin/dashboard')
      else router.replace('/')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur de connexion')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Connexion</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="email">Email</Label>
            <Input id="email" name="email" type="email" required />
          </div>
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="motDePasse">Mot de passe</Label>
            <Input id="motDePasse" name="motDePasse" type="password" required />
          </div>
          <Button type="submit" disabled={loading}>
            {loading ? 'Connexion…' : 'Se connecter'}
          </Button>
          <div className="text-sm text-center text-muted-foreground">
            Pas de compte ?{' '}
            <Link href="/inscription/candidat" className="underline">Candidat</Link>
            {' · '}
            <Link href="/inscription/entreprise" className="underline">Entreprise</Link>
          </div>
        </form>
      </CardContent>
    </Card>
  )
}
```

- [ ] **Step 3: Inscription candidat page**

Create `frontend/app/(auth)/inscription/candidat/page.tsx`:

```tsx
'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { apiFetch } from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription } from '@/components/ui/alert'
import Link from 'next/link'

export default function InscriptionCandidatPage() {
  const router = useRouter()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch('/api/identite/candidats/inscription', {
        method: 'POST',
        body: JSON.stringify({
          prenom: fd.get('prenom'),
          nom: fd.get('nom'),
          email: fd.get('email'),
          motDePasse: fd.get('motDePasse'),
        }),
      })
      router.replace('/connexion')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de l\'inscription')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Inscription Candidat</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="prenom">Prénom</Label>
            <Input id="prenom" name="prenom" required />
          </div>
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="nom">Nom</Label>
            <Input id="nom" name="nom" required />
          </div>
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="email">Email</Label>
            <Input id="email" name="email" type="email" required />
          </div>
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="motDePasse">Mot de passe</Label>
            <Input id="motDePasse" name="motDePasse" type="password" required />
          </div>
          <Button type="submit" disabled={loading}>
            {loading ? 'Inscription…' : 'Créer mon compte'}
          </Button>
          <p className="text-sm text-center text-muted-foreground">
            Déjà inscrit ? <Link href="/connexion" className="underline">Se connecter</Link>
          </p>
        </form>
      </CardContent>
    </Card>
  )
}
```

- [ ] **Step 4: Inscription entreprise page**

Create `frontend/app/(auth)/inscription/entreprise/page.tsx`:

```tsx
'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { apiFetch } from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription } from '@/components/ui/alert'
import Link from 'next/link'

export default function InscriptionEntreprisePage() {
  const router = useRouter()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch('/api/identite/entreprises/inscription', {
        method: 'POST',
        body: JSON.stringify({
          raisonSociale: fd.get('raisonSociale'),
          siret: fd.get('siret'),
          email: fd.get('email'),
          motDePasse: fd.get('motDePasse'),
        }),
      })
      router.replace('/connexion')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de l\'inscription')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Inscription Entreprise</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="raisonSociale">Raison sociale</Label>
            <Input id="raisonSociale" name="raisonSociale" required />
          </div>
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="siret">SIRET (14 chiffres)</Label>
            <Input id="siret" name="siret" maxLength={14} required />
          </div>
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="email">Email</Label>
            <Input id="email" name="email" type="email" required />
          </div>
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="motDePasse">Mot de passe</Label>
            <Input id="motDePasse" name="motDePasse" type="password" required />
          </div>
          <Button type="submit" disabled={loading}>
            {loading ? 'Inscription…' : 'Créer mon compte'}
          </Button>
          <p className="text-sm text-center text-muted-foreground">
            Déjà inscrit ? <Link href="/connexion" className="underline">Se connecter</Link>
          </p>
        </form>
      </CardContent>
    </Card>
  )
}
```

---

## Task 8: Public pages (annonces + appels d'offre)

**Files:**
- Create: `frontend/app/(public)/layout.tsx`
- Create: `frontend/app/(public)/annonces/page.tsx`
- Create: `frontend/app/(public)/appels-offre/page.tsx`

- [ ] **Step 1: Public layout with navbar**

```tsx
// frontend/app/(public)/layout.tsx
'use client'

import Link from 'next/link'
import { useEffect, useState } from 'react'
import { getAuth, logout } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import { useRouter } from 'next/navigation'

export default function PublicLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter()
  const [role, setRole] = useState<string | null>(null)

  useEffect(() => {
    setRole(getAuth()?.role ?? null)
  }, [])

  function handleLogout() {
    logout()
    router.replace('/connexion')
  }

  const dashboardHref =
    role === 'Candidat' ? '/candidat/dashboard'
    : role === 'Entreprise' ? '/entreprise/dashboard'
    : role === 'Administrateur' ? '/admin/dashboard'
    : null

  return (
    <div className="min-h-screen flex flex-col">
      <header className="border-b bg-background px-6 py-3 flex items-center justify-between">
        <nav className="flex items-center gap-6">
          <Link href="/" className="font-bold text-lg">CVTech</Link>
          <Link href="/annonces" className="text-sm text-muted-foreground hover:text-foreground">Annonces</Link>
          <Link href="/appels-offre" className="text-sm text-muted-foreground hover:text-foreground">Appels d&apos;offre</Link>
        </nav>
        <div className="flex items-center gap-2">
          {dashboardHref ? (
            <>
              <Button variant="outline" size="sm" asChild>
                <Link href={dashboardHref}>Mon espace</Link>
              </Button>
              <Button variant="ghost" size="sm" onClick={handleLogout}>Déconnexion</Button>
            </>
          ) : (
            <>
              <Button variant="outline" size="sm" asChild>
                <Link href="/connexion">Se connecter</Link>
              </Button>
              <Button size="sm" asChild>
                <Link href="/inscription/candidat">S&apos;inscrire</Link>
              </Button>
            </>
          )}
        </div>
      </header>
      <main className="flex-1 container mx-auto py-8 px-4">{children}</main>
    </div>
  )
}
```

- [ ] **Step 2: Annonces listing page**

Create `frontend/app/(public)/annonces/page.tsx`:

```tsx
'use client'

import { useEffect, useState } from 'react'
import { apiFetch } from '@/lib/api'
import { getAuth } from '@/lib/auth'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { useRouter } from 'next/navigation'

interface Annonce {
  id: string
  titre: string
  description: string
  domaineMetier: string
  typeContrat: string
  datePublication: string
}

function PostulerDialog({ annonceId }: { annonceId: string }) {
  const [open, setOpen] = useState(false)
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState('')
  const auth = getAuth()
  const router = useRouter()

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch(`/api/annonces/${annonceId}/candidatures`, {
        method: 'POST',
        body: JSON.stringify({
          curriculumVitaeId: crypto.randomUUID(),
          lettreMotivation: fd.get('lettreMotivation') || null,
        }),
      })
      setSuccess(true)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la candidature')
    } finally {
      setLoading(false)
    }
  }

  function handleOpenChange(val: boolean) {
    if (!auth || auth.role !== 'Candidat') {
      router.push(`/connexion?redirect=/annonces`)
      return
    }
    setOpen(val)
    if (!val) { setSuccess(false); setError('') }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button size="sm">Postuler</Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Envoyer ma candidature</DialogTitle>
        </DialogHeader>
        {success ? (
          <Alert><AlertDescription>Candidature envoyée avec succès !</AlertDescription></Alert>
        ) : (
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="lettreMotivation">Lettre de motivation (optionnelle)</Label>
              <Textarea id="lettreMotivation" name="lettreMotivation" rows={4} />
            </div>
            <Button type="submit" disabled={loading}>
              {loading ? 'Envoi…' : 'Envoyer ma candidature'}
            </Button>
          </form>
        )}
      </DialogContent>
    </Dialog>
  )
}

export default function AnnoncesPage() {
  const [annonces, setAnnonces] = useState<Annonce[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    apiFetch<Annonce[]>('/api/annonces')
      .then(setAnnonces)
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <p className="text-muted-foreground">Chargement…</p>

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold">Offres d&apos;emploi</h1>
      {annonces.length === 0 && (
        <p className="text-muted-foreground">Aucune annonce pour le moment.</p>
      )}
      <div className="grid gap-4">
        {annonces.map(a => (
          <Card key={a.id}>
            <CardHeader className="flex flex-row items-start justify-between gap-4">
              <div>
                <CardTitle className="text-lg">{a.titre}</CardTitle>
                <div className="flex gap-2 mt-1">
                  <Badge variant="secondary">{a.typeContrat}</Badge>
                  <Badge variant="outline">{a.domaineMetier}</Badge>
                </div>
              </div>
              <PostulerDialog annonceId={a.id} />
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground">{a.description}</p>
              <p className="text-xs text-muted-foreground mt-2">
                Publiée le {new Date(a.datePublication).toLocaleDateString('fr-FR')}
              </p>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  )
}
```

- [ ] **Step 3: Appels d'offre listing page**

Create `frontend/app/(public)/appels-offre/page.tsx`:

```tsx
'use client'

import { useEffect, useState } from 'react'
import { apiFetch } from '@/lib/api'
import { getAuth } from '@/lib/auth'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog'
import { Label } from '@/components/ui/label'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { useRouter } from 'next/navigation'

interface AppelOffre {
  id: string
  titre: string
  contexte: string
  domaineMetier: string
  budgetMax: number
  deadline: string
  datePublication: string
}

function SoumettreDialog({ appelOffreId }: { appelOffreId: string }) {
  const [open, setOpen] = useState(false)
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState('')
  const auth = getAuth()
  const router = useRouter()

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch(`/api/appels-offre/${appelOffreId}/propositions`, {
        method: 'POST',
        body: JSON.stringify({
          tauxJournalierMoyen: Number(fd.get('tjm')),
          dureeEstimeeJours: Number(fd.get('duree')),
          methodologie: fd.get('methodologie'),
        }),
      })
      setSuccess(true)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la soumission')
    } finally {
      setLoading(false)
    }
  }

  function handleOpenChange(val: boolean) {
    if (!auth || auth.role !== 'Candidat') {
      router.push(`/connexion?redirect=/appels-offre`)
      return
    }
    setOpen(val)
    if (!val) { setSuccess(false); setError('') }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button size="sm">Soumettre une proposition</Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Ma proposition</DialogTitle>
        </DialogHeader>
        {success ? (
          <Alert><AlertDescription>Proposition soumise avec succès !</AlertDescription></Alert>
        ) : (
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="tjm">TJM (€/jour)</Label>
              <Input id="tjm" name="tjm" type="number" min="1" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="duree">Durée estimée (jours)</Label>
              <Input id="duree" name="duree" type="number" min="1" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="methodologie">Méthodologie</Label>
              <Textarea id="methodologie" name="methodologie" rows={4} required />
            </div>
            <Button type="submit" disabled={loading}>
              {loading ? 'Envoi…' : 'Soumettre ma proposition'}
            </Button>
          </form>
        )}
      </DialogContent>
    </Dialog>
  )
}

export default function AppelsOffrePage() {
  const [appels, setAppels] = useState<AppelOffre[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    apiFetch<AppelOffre[]>('/api/appels-offre')
      .then(setAppels)
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <p className="text-muted-foreground">Chargement…</p>

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold">Appels d&apos;offre freelance</h1>
      {appels.length === 0 && (
        <p className="text-muted-foreground">Aucun appel d&apos;offre pour le moment.</p>
      )}
      <div className="grid gap-4">
        {appels.map(a => (
          <Card key={a.id}>
            <CardHeader className="flex flex-row items-start justify-between gap-4">
              <div>
                <CardTitle className="text-lg">{a.titre}</CardTitle>
                <div className="flex gap-2 mt-1">
                  <Badge variant="secondary">Budget max: {a.budgetMax.toLocaleString('fr-FR')} €</Badge>
                  <Badge variant="outline">{a.domaineMetier}</Badge>
                </div>
              </div>
              <SoumettreDialog appelOffreId={a.id} />
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground">{a.contexte}</p>
              <p className="text-xs text-muted-foreground mt-2">
                Deadline : {new Date(a.deadline).toLocaleDateString('fr-FR')} ·
                Publié le {new Date(a.datePublication).toLocaleDateString('fr-FR')}
              </p>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  )
}
```

---

## Task 9: Candidat pages

**Files:**
- Create: `frontend/app/(candidat)/layout.tsx`
- Create: `frontend/app/(candidat)/dashboard/page.tsx`
- Create: `frontend/app/(candidat)/abonnements/page.tsx`

- [ ] **Step 1: Candidat layout**

```tsx
// frontend/app/(candidat)/layout.tsx
'use client'

import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { logout } from '@/lib/auth'
import { Button } from '@/components/ui/button'

export default function CandidatLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter()

  function handleLogout() {
    logout()
    router.replace('/connexion')
  }

  return (
    <div className="min-h-screen flex">
      <aside className="w-52 border-r bg-muted/30 flex flex-col p-4 gap-2">
        <Link href="/" className="font-bold text-lg mb-4">CVTech</Link>
        <Link href="/candidat/dashboard" className="text-sm hover:underline">Tableau de bord</Link>
        <Link href="/annonces" className="text-sm hover:underline">Parcourir les annonces</Link>
        <Link href="/appels-offre" className="text-sm hover:underline">Appels d&apos;offre</Link>
        <Link href="/candidat/abonnements" className="text-sm hover:underline">Mes abonnements</Link>
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

- [ ] **Step 2: Candidat dashboard**

```tsx
// frontend/app/(candidat)/dashboard/page.tsx
'use client'

import { useEffect, useState } from 'react'
import { getAuth } from '@/lib/auth'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import Link from 'next/link'

export default function CandidatDashboard() {
  const [auth, setAuth] = useState<{ utilisateurId: string; role: string } | null>(null)

  useEffect(() => {
    setAuth(getAuth())
  }, [])

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold">Tableau de bord Candidat</h1>
      <p className="text-muted-foreground">Bienvenue sur votre espace candidat.</p>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Card>
          <CardHeader><CardTitle>Offres d&apos;emploi</CardTitle></CardHeader>
          <CardContent className="flex flex-col gap-2">
            <p className="text-sm text-muted-foreground">Parcourez les annonces et postulez en un clic.</p>
            <Button asChild><Link href="/annonces">Voir les annonces</Link></Button>
          </CardContent>
        </Card>
        <Card>
          <CardHeader><CardTitle>Missions freelance</CardTitle></CardHeader>
          <CardContent className="flex flex-col gap-2">
            <p className="text-sm text-muted-foreground">Répondez aux appels d&apos;offre freelance.</p>
            <Button asChild><Link href="/appels-offre">Voir les appels d&apos;offre</Link></Button>
          </CardContent>
        </Card>
        <Card>
          <CardHeader><CardTitle>Abonnements</CardTitle></CardHeader>
          <CardContent className="flex flex-col gap-2">
            <p className="text-sm text-muted-foreground">Recevez des notifications par domaine métier.</p>
            <Button variant="outline" asChild><Link href="/candidat/abonnements">Gérer mes abonnements</Link></Button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
```

- [ ] **Step 3: Abonnements page**

```tsx
// frontend/app/(candidat)/abonnements/page.tsx
'use client'

import { useState } from 'react'
import { apiFetch } from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'

export default function AbonnementsPage() {
  const [canal, setCanal] = useState<'Email' | 'InApp'>('Email')
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState('')

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
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de l\'abonnement')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex flex-col gap-6 max-w-md">
      <h1 className="text-2xl font-bold">Mes abonnements</h1>
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
              {loading ? 'Abonnement…' : 'S\'abonner'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
```

---

## Task 10: Entreprise pages

**Files:**
- Create: `frontend/app/(entreprise)/layout.tsx`
- Create: `frontend/app/(entreprise)/dashboard/page.tsx`
- Create: `frontend/app/(entreprise)/annonces/publier/page.tsx`
- Create: `frontend/app/(entreprise)/appels-offre/publier/page.tsx`

- [ ] **Step 1: Entreprise layout**

```tsx
// frontend/app/(entreprise)/layout.tsx
'use client'

import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { logout } from '@/lib/auth'
import { Button } from '@/components/ui/button'

export default function EntrepriseLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter()

  function handleLogout() {
    logout()
    router.replace('/connexion')
  }

  return (
    <div className="min-h-screen flex">
      <aside className="w-52 border-r bg-muted/30 flex flex-col p-4 gap-2">
        <Link href="/" className="font-bold text-lg mb-4">CVTech</Link>
        <Link href="/entreprise/dashboard" className="text-sm hover:underline">Tableau de bord</Link>
        <Link href="/entreprise/annonces/publier" className="text-sm hover:underline">Publier une annonce</Link>
        <Link href="/entreprise/appels-offre/publier" className="text-sm hover:underline">Publier un appel d&apos;offre</Link>
        <Link href="/annonces" className="text-sm hover:underline">Voir toutes les annonces</Link>
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

- [ ] **Step 2: Entreprise dashboard**

```tsx
// frontend/app/(entreprise)/dashboard/page.tsx
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import Link from 'next/link'

export default function EntrepriseDashboard() {
  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold">Tableau de bord Entreprise</h1>
      <p className="text-muted-foreground">Gérez vos offres d&apos;emploi et appels d&apos;offre freelance.</p>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Card>
          <CardHeader><CardTitle>Annonces d&apos;emploi</CardTitle></CardHeader>
          <CardContent className="flex flex-col gap-2">
            <p className="text-sm text-muted-foreground">Publiez une nouvelle offre d&apos;emploi (CDI, CDD, stage…).</p>
            <Button asChild><Link href="/entreprise/annonces/publier">Publier une annonce</Link></Button>
          </CardContent>
        </Card>
        <Card>
          <CardHeader><CardTitle>Appels d&apos;offre freelance</CardTitle></CardHeader>
          <CardContent className="flex flex-col gap-2">
            <p className="text-sm text-muted-foreground">Publiez un cahier des charges pour une mission ponctuelle.</p>
            <Button asChild><Link href="/entreprise/appels-offre/publier">Publier un appel d&apos;offre</Link></Button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
```

- [ ] **Step 3: Publier annonce page**

```tsx
// frontend/app/(entreprise)/annonces/publier/page.tsx
'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { apiFetch } from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'

const TYPE_CONTRATS = ['Cdi', 'Cdd', 'Stage', 'Alternance', 'Apprentissage'] as const
type TypeContrat = typeof TYPE_CONTRATS[number]

export default function PublierAnnoncePage() {
  const router = useRouter()
  const [typeContrat, setTypeContrat] = useState<TypeContrat>('Cdi')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch('/api/annonces', {
        method: 'POST',
        body: JSON.stringify({
          titre: fd.get('titre'),
          description: fd.get('description'),
          domaineMetier: fd.get('domaineMetier'),
          typeContrat,
        }),
      })
      router.push('/annonces')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la publication')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-lg">
      <h1 className="text-2xl font-bold mb-6">Publier une annonce d&apos;emploi</h1>
      <Card>
        <CardContent className="pt-6">
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="titre">Titre du poste</Label>
              <Input id="titre" name="titre" placeholder="ex: Développeur .NET Senior" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="description">Description</Label>
              <Textarea id="description" name="description" rows={5} required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="domaineMetier">Domaine métier</Label>
              <Input id="domaineMetier" name="domaineMetier" placeholder="ex: Informatique, DevOps…" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label>Type de contrat</Label>
              <Select value={typeContrat} onValueChange={(v) => setTypeContrat(v as TypeContrat)}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {TYPE_CONTRATS.map(t => (
                    <SelectItem key={t} value={t}>{t}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <Button type="submit" disabled={loading}>
              {loading ? 'Publication…' : 'Publier l\'annonce'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
```

- [ ] **Step 4: Publier appel d'offre page**

```tsx
// frontend/app/(entreprise)/appels-offre/publier/page.tsx
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

export default function PublierAppelOffrePage() {
  const router = useRouter()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const fd = new FormData(e.currentTarget)
    try {
      await apiFetch('/api/appels-offre', {
        method: 'POST',
        body: JSON.stringify({
          titre: fd.get('titre'),
          contexte: fd.get('contexte'),
          domaineMetier: fd.get('domaineMetier'),
          budgetMax: Number(fd.get('budgetMax')),
          deadline: new Date(fd.get('deadline') as string).toISOString(),
        }),
      })
      router.push('/appels-offre')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la publication')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-lg">
      <h1 className="text-2xl font-bold mb-6">Publier un appel d&apos;offre freelance</h1>
      <Card>
        <CardContent className="pt-6">
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="titre">Titre de la mission</Label>
              <Input id="titre" name="titre" placeholder="ex: Refonte API REST" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="contexte">Contexte / cahier des charges</Label>
              <Textarea id="contexte" name="contexte" rows={5} required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="domaineMetier">Domaine métier</Label>
              <Input id="domaineMetier" name="domaineMetier" placeholder="ex: Backend, Cloud…" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="budgetMax">Budget maximum (€)</Label>
              <Input id="budgetMax" name="budgetMax" type="number" min="1" required />
            </div>
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="deadline">Deadline</Label>
              <Input id="deadline" name="deadline" type="date" required />
            </div>
            <Button type="submit" disabled={loading}>
              {loading ? 'Publication…' : 'Publier l\'appel d\'offre'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
```

---

## Task 11: Admin page

**Files:**
- Create: `frontend/app/(admin)/layout.tsx`
- Create: `frontend/app/(admin)/dashboard/page.tsx`

- [ ] **Step 1: Admin layout**

```tsx
// frontend/app/(admin)/layout.tsx
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
      <aside className="w-52 border-r bg-muted/30 flex flex-col p-4 gap-2">
        <Link href="/" className="font-bold text-lg mb-4">CVTech Admin</Link>
        <Link href="/admin/dashboard" className="text-sm hover:underline">Tableau de bord</Link>
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

- [ ] **Step 2: Admin dashboard ("En construction")**

```tsx
// frontend/app/(admin)/dashboard/page.tsx
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'

export default function AdminDashboard() {
  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center gap-3">
        <h1 className="text-2xl font-bold">Console Administration</h1>
        <Badge variant="secondary">En construction</Badge>
      </div>
      <Card className="max-w-md">
        <CardHeader><CardTitle>Fonctionnalités prévues</CardTitle></CardHeader>
        <CardContent className="text-sm text-muted-foreground flex flex-col gap-2">
          <p>• Modération des annonces et appels d&apos;offre</p>
          <p>• Publication d&apos;articles dans le fil d&apos;actualité</p>
          <p>• Blocage / réactivation de comptes</p>
          <p>• Gestion du référentiel des domaines métier</p>
        </CardContent>
      </Card>
    </div>
  )
}
```

---

## Task 12: End-to-end smoke test

- [ ] **Step 1: Ensure API is running in Development mode**

```bash
lsof -i :5000 | grep LISTEN
```

Expected: `CVTech.Ap` process listening. If not:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/CVTech.Api/CVTech.Api.csproj &
sleep 5
```

- [ ] **Step 2: Start Next.js dev server**

```bash
cd frontend && npm run dev &
sleep 5
python3 -c "import urllib.request; r = urllib.request.urlopen('http://localhost:3000'); print('Frontend:', r.status)"
```

Expected: `Frontend: 200`

- [ ] **Step 3: Verify public endpoints load**

```bash
python3 -c "
import urllib.request, json
r = urllib.request.urlopen('http://localhost:3000/annonces')
print('annonces page:', r.status)
r2 = urllib.request.urlopen('http://localhost:3000/appels-offre')
print('appels-offre page:', r2.status)
"
```

Expected: Both `200`.

- [ ] **Step 4: Verify auth redirect works**

```bash
python3 -c "
import urllib.request
try:
    urllib.request.urlopen('http://localhost:3000/candidat/dashboard')
    print('ERROR: should have redirected')
except Exception as e:
    print('Redirected (expected):', type(e).__name__)
"
```

Expected: redirect or error (middleware blocks unauthenticated access).

- [ ] **Step 5: Manual browser test — Parcours Candidat**

Open `http://localhost:3000` in a browser and verify:
1. Home page shows with "Voir les annonces" and "Se connecter" buttons
2. `/inscription/candidat` — create a new account → redirects to `/connexion`
3. `/connexion` — login → redirects to `/candidat/dashboard`
4. `/annonces` — list visible, "Postuler" opens modal
5. `/candidat/abonnements` — subscribe form works
6. Déconnexion → back to `/connexion`

- [ ] **Step 6: Manual browser test — Parcours Entreprise**

1. `/inscription/entreprise` — create account
2. `/connexion` — login → redirects to `/entreprise/dashboard`
3. `/entreprise/annonces/publier` — publish a job listing → redirects to `/annonces`
4. `/annonces` — new listing visible
5. `/entreprise/appels-offre/publier` — publish RFP → redirects to `/appels-offre`
6. `/appels-offre` — new RFP visible

- [ ] **Step 7: Manual browser test — Parcours Admin**

1. Create admin account directly via API (no frontend for admin registration):

```bash
python3 -c "
import urllib.request, json
# Use existing admin if available, or use the API to check
data = json.dumps({'email':'admin@cvtech.com','motDePasse':'Admin1234!'}).encode()
req = urllib.request.Request('http://localhost:5000/api/identite/connexion', data=data, headers={'Content-Type':'application/json'})
try:
    resp = urllib.request.urlopen(req)
    body = json.loads(resp.read().decode())
    print('Admin token role:', body['role'])
except Exception as e:
    print('No admin account yet:', e)
"
```

Note: If no admin account exists, the admin parcours can be demonstrated by noting it's protected and shows "En construction". Admin account creation requires direct DB seeding (no public endpoint).

- [ ] **Step 8: Verify TypeScript build passes**

```bash
cd frontend && npx tsc --noEmit
```

Expected: No errors.

---

## Self-Review Notes

**Spec coverage check:**
- ✅ 3 user journeys: Candidat, Entreprise, Admin ("En construction")
- ✅ Public pages: annonces + appels-offre (no auth required)
- ✅ Auth: connexion + inscription candidat/entreprise
- ✅ JWT in localStorage + cookie for middleware
- ✅ Route protection by role via middleware
- ✅ Candidat: postuler (modal), abonnements
- ✅ Entreprise: publier annonce, publier appel d'offre
- ✅ API GET endpoints added (Task 1 + 2)
- ✅ `CurriculumVitaeId` handled via `crypto.randomUUID()` (no CV management scope)
- ✅ shadcn/ui components throughout

**Known simplifications:**
- Admin parcours: "En construction" — no API endpoints exist for admin actions
- No CV management flow — `curriculumVitaeId` is generated client-side
- No "mes candidatures" / "mes propositions" listing — no GET-by-user endpoints in API
- Admin account creation requires direct DB seeding (no public registration endpoint for Administrateur role)

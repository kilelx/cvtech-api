# Frontend CVTech — Design Spec

## Stack
- Next.js 14+ App Router, TypeScript
- shadcn/ui pour les composants
- Pas de state manager externe (useState + fetch)
- JWT stocké dans localStorage + cookie (pour middleware)

## Structure

```
frontend/
├── app/
│   ├── (public)/
│   │   ├── annonces/page.tsx
│   │   ├── appels-offre/page.tsx
│   │   └── layout.tsx
│   ├── (auth)/
│   │   ├── connexion/page.tsx
│   │   ├── inscription/candidat/page.tsx
│   │   ├── inscription/entreprise/page.tsx
│   │   └── layout.tsx
│   ├── (candidat)/
│   │   ├── dashboard/page.tsx
│   │   ├── abonnements/page.tsx
│   │   └── layout.tsx
│   ├── (entreprise)/
│   │   ├── dashboard/page.tsx
│   │   ├── annonces/publier/page.tsx
│   │   ├── appels-offre/publier/page.tsx
│   │   └── layout.tsx
│   ├── (admin)/
│   │   ├── dashboard/page.tsx
│   │   └── layout.tsx
│   ├── layout.tsx
│   └── page.tsx
├── lib/
│   ├── api.ts
│   └── auth.ts
├── middleware.ts
└── components/ui/
```

## Pages

| Route | Accès | Contenu |
|-------|-------|---------|
| `/` | public | redirect selon rôle |
| `/annonces` | public | liste annonces, bouton Postuler (modal si auth) |
| `/appels-offre` | public | liste AO, bouton Soumettre (modal si auth) |
| `/connexion` | public | form email + mdp |
| `/inscription/candidat` | public | form prénom, nom, email, mdp |
| `/inscription/entreprise` | public | form raison sociale, SIRET, email, mdp |
| `/candidat/dashboard` | Candidat | mes candidatures + propositions soumises |
| `/candidat/abonnements` | Candidat | s'abonner à des domaines métier |
| `/entreprise/dashboard` | Entreprise | mes annonces + AO publiés |
| `/entreprise/annonces/publier` | Entreprise | form publier annonce emploi |
| `/entreprise/appels-offre/publier` | Entreprise | form publier appel d'offre |
| `/admin/dashboard` | Admin | "En construction" |

## Auth

- Connexion → `{ token, role, utilisateurId }` → localStorage + cookie `token`
- `lib/auth.ts` : `getToken()`, `getRole()` (decode JWT payload base64), `logout()`
- `middleware.ts` : lit cookie `token`, protège routes par rôle, redirige `/connexion` si absent
- Erreur 401 depuis l'API → `logout()` + redirect `/connexion`

## Data flow

- `lib/api.ts` : wrapper fetch, injecte `Authorization: Bearer <token>` si token présent
- Pages publiques : fetch sans token (annonces/AO visibles par tous)
- Bouton "Postuler"/"Soumettre" sur page publique → si non connecté, redirect `/connexion?redirect=<url>`, après connexion redirect back + modal shadcn

## Modifications API requises

Ajouter avant le frontend :
- `GET /api/annonces` → liste publique des annonces emploi
- `GET /api/appels-offre` → liste publique des appels d'offre

## Parcours Admin

Page `/admin/dashboard` affiche "En construction" — pas d'endpoints API admin disponibles.

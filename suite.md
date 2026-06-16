Après le plan executé, reste 4 gros blocs:

1. API host (manque encore)
   — src/CVTech.Api/Program.cs + CVTech.Api.csproj qui wire les 4 ModuleLoader, JWT middleware, CORS. Sans ça rien ne
   tourne.

2. EF Core migrations
   — dotnet ef migrations add Init par DbContext (4 migrations), script SQL pour Azure.

3. Frontend (le plus gros)
   — React ou Blazor WASM, 3 parcours utilisateur distincts (Candidat / Entreprise / Admin). C'est évalué sur la
   fonctionnalité, pas juste la structure.

4. README.md + Mermaid
   — Le diagramme de lancement est obligatoire dans les livrables, l'enseignant l'utilise pour valider la DX.

Ordre logique:
scaffold terminé → API host → migrations → test end-to-end local → frontend → README

Frontend est le gros inconnu — React ou Blazor, t'as déjà une préférence?

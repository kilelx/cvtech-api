---
name: cvtech-rss-guard
description: Impose les contraintes du flux RSS éditorial — contenu exclusivement éditorial, format RSS 2.0 valide, jamais d'annonces d'emploi ni d'appels d'offre
globs: "src/Modules/ActualiteEtAbonnement/**/*.cs"
---
# CONTEXTE
Le module `ActualiteEtAbonnement` expose un endpoint RSS public `/feed/rss`. Ce flux est STRICTEMENT réservé aux articles éditoriaux publiés par les administrateurs. Il ne contient ni annonces d'emploi (module CatalogueEmploi) ni appels d'offre (module AppelOffreFreelance).

# RÈGLES STRICTES DU FLUX RSS

## 1. CONTENU AUTORISÉ — UNIQUEMENT LES ARTICLES ÉDITORIAUX
*   Le flux RSS ne sérialise que des entités de type `ArticleActualite`.
*   **INTERDIT** d'inclure `AnnonceEmploi`, `AppelOffre`, ou tout autre agrégat d'un autre module.
*   Si une demande IA tente d'ajouter des annonces au flux RSS, **rejeter et expliquer la règle**.

## 2. FORMAT RSS 2.0 OBLIGATOIRE
*   L'endpoint `GET /feed/rss` retourne un `ContentResult` avec `ContentType = "application/rss+xml; charset=utf-8"`.
*   La structure XML doit être un RSS 2.0 valide (validateur W3C : https://validator.w3.org/feed/).
*   Éléments obligatoires par `<item>` : `<title>`, `<link>`, `<description>`, `<pubDate>` (format RFC 822), `<guid>`.

```csharp
// Exemple de structure minimale valide
var rss = new XDocument(
    new XElement("rss", new XAttribute("version", "2.0"),
        new XElement("channel",
            new XElement("title", "CVTech — Actualités Tech"),
            new XElement("link", baseUrl),
            new XElement("description", "Fil éditorial de la plateforme CVTech"),
            articles.Select(a => new XElement("item",
                new XElement("title", a.Titre),
                new XElement("link", $"{baseUrl}/articles/{a.Id}"),
                new XElement("description", a.Resume),
                new XElement("pubDate", a.DatePublication.ToString("R")),
                new XElement("guid", a.Id.ToString())
            ))
        )
    )
);
```

## 3. ENDPOINT ET FILTRAGE
*   **Route principale** : `GET /feed/rss` — tous les articles éditoriaux publiés, ordre chronologique inverse.
*   **Filtrage optionnel** : `GET /feed/rss?domaine=cloud-azure` — filtre par `DomaineMetier` de l'article.
*   L'endpoint est **anonyme** : aucune authentification requise, aucun middleware d'autorisation.

## 4. ACCÈS AUX DONNÉES — ISOLATION RESPECTÉE
*   Le Handler RSS lit uniquement la table/DbSet `ArticlesActualite` du DbContext du module `ActualiteEtAbonnement`.
*   Il ne fait aucune jointure ni requête vers les tables des autres modules.

## 5. PUBLICATION DES ARTICLES
*   Seul un utilisateur avec le rôle `Administrateur` peut créer un `ArticleActualite` (vérification via `IVerificateurPermission` — cf. skill `cvtech-permission-guard`).
*   La publication déclenche aucun événement inter-modules (le RSS est un fil pull, pas push).

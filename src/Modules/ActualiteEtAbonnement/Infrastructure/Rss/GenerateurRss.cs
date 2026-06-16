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

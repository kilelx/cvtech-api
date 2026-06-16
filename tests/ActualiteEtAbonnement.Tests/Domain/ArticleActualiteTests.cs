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

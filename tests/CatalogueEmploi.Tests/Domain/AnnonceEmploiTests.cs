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

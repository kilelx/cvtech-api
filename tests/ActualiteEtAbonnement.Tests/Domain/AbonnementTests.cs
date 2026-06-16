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
}

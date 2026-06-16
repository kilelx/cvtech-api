using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;
using FluentAssertions;

namespace AppelOffreFreelance.Tests.Domain;

public class AppelOffreTests
{
    [Fact]
    public void UnAppelOffre_QuandCree_ALeStatutOuvert()
    {
        var ao = AppelOffre.Creer("Mission Cloud", "Contexte", "cloud-azure", 50000m, DateTime.UtcNow.AddMonths(1), Guid.NewGuid());

        ao.Statut.Should().Be(StatutAppelOffre.Ouvert, "un appel d'offre nouvellement créé est ouvert");
    }

    [Fact]
    public void UnAppelOffre_QuandClos_NAcceptePlusDePropositions()
    {
        var ao = AppelOffre.Creer("Mission Cloud", "Contexte", "cloud-azure", 50000m, DateTime.UtcNow.AddMonths(1), Guid.NewGuid());

        ao.Clore();

        ao.Statut.Should().Be(StatutAppelOffre.Clos, "un appel d'offre clos ne peut plus recevoir de propositions");
    }

    [Fact]
    public void UnePropositionFreelance_QuandCreee_ConserveLesTjmEtDuree()
    {
        var proposition = PropositionFreelance.Creer(Guid.NewGuid(), Guid.NewGuid(), 650m, 20, "Méthode agile");

        proposition.TauxJournalierMoyen.Should().Be(650m);
        proposition.DureeEstimeeJours.Should().Be(20);
    }
}

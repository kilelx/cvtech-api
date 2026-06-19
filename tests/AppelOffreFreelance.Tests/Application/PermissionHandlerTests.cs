using AppelOffreFreelance.Tests.Fakes;
using CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;
using CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;
using CVTech.Modules.AppelOffreFreelance.Application.Features.SupprimerAppelOffre;
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.GestionIdentite.Contracts;
using FluentAssertions;

namespace AppelOffreFreelance.Tests.Application;

public class PermissionHandlerTests
{
    [Fact]
    public async Task UnCandidatNePeutPasPublierUnAppelOffre()
    {
        var handler = new PublierAppelOffreCommandHandler(
            new FakeAppelOffreRepository(),
            new FakeVerificateurPermission(autorise: false),
            new FakePublisher()
        );
        var command = new PublierAppelOffreCommand(
            Guid.NewGuid(), "Mission Cloud", "Contexte", "cloud-azure", 50000m, DateTime.UtcNow.AddMonths(1)
        );

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }

    [Fact]
    public async Task UneEntrepriseNePeutPasSoumettreUneProposition()
    {
        var handler = new SoumettrePropositionCommandHandler(
            new FakeAppelOffreRepository(),
            new FakePropositionRepository(),
            new FakeVerificateurPermission(autorise: false)
        );
        var command = new SoumettrePropositionCommand(Guid.NewGuid(), Guid.NewGuid(), 650m, 20, "Agile");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }

    [Fact]
    public async Task UnNonAdminNePeutPasSupprimerUnAppelOffre()
    {
        var repo = new FakeAppelOffreRepository();
        var ao = AppelOffre.Creer("Mission", "Contexte", "cloud", 10000m, DateTime.UtcNow.AddMonths(1), Guid.NewGuid());
        repo.Ajouter(ao);

        var handler = new SupprimerAppelOffreCommandHandler(
            repo,
            new FakeVerificateurPermission(autorise: false)
        );
        var command = new SupprimerAppelOffreCommand(Guid.NewGuid(), ao.Id);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }
}

using CatalogueEmploi.Tests.Fakes;
using CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;
using CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;
using CVTech.Modules.CatalogueEmploi.Application.Features.SupprimerAnnonce;
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Enums;
using CVTech.Modules.GestionIdentite.Contracts;
using FluentAssertions;

namespace CatalogueEmploi.Tests.Application;

public class PermissionHandlerTests
{
    [Fact]
    public async Task UnCandidatNePeutPasPublierUneAnnonce()
    {
        var handler = new PublierAnnonceCommandHandler(
            new FakeAnnonceRepository(),
            new FakeVerificateurPermission(autorise: false),
            new FakePublisher()
        );
        var command = new PublierAnnonceCommand(Guid.NewGuid(), "Dev C#", "Mission test", "dotnet", TypeContrat.Cdi);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }

    [Fact]
    public async Task UneEntrepriseNePeutPasPostulerAUneAnnonce()
    {
        var handler = new PostulerAnnonceCommandHandler(
            new FakeAnnonceRepository(),
            new FakeCandidatureRepository(),
            new FakeVerificateurPermission(autorise: false)
        );
        var command = new PostulerAnnonceCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }

    [Fact]
    public async Task UnNonAdminNePeutPasSupprimerUneAnnonce()
    {
        var repo = new FakeAnnonceRepository();
        var annonce = AnnonceEmploi.Creer("Dev C#", "Mission test", "dotnet", TypeContrat.Cdi, Guid.NewGuid());
        repo.Ajouter(annonce);

        var handler = new SupprimerAnnonceCommandHandler(
            repo,
            new FakeVerificateurPermission(autorise: false)
        );
        var command = new SupprimerAnnonceCommand(Guid.NewGuid(), annonce.Id);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }
}

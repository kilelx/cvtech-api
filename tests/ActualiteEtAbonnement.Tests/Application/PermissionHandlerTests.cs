using ActualiteEtAbonnement.Tests.Fakes;
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.PublierArticle;
using CVTech.Modules.GestionIdentite.Contracts;
using FluentAssertions;

namespace ActualiteEtAbonnement.Tests.Application;

public class PermissionHandlerTests
{
    [Fact]
    public async Task UnCandidatNePeutPasPublierUnArticle()
    {
        var handler = new PublierArticleCommandHandler(
            new FakeArticleRepository(),
            new FakeVerificateurPermission(autorise: false)
        );
        var command = new PublierArticleCommand(Guid.NewGuid(), "Titre test", "Contenu test", null);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<PermissionRefuseeException>();
    }
}

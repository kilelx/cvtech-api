using CVTech.Modules.GestionIdentite.Contracts;

namespace AppelOffreFreelance.Tests.Fakes;

public class FakeVerificateurPermission(bool autorise) : IVerificateurPermission
{
    public Task<bool> AutoriserAsync(Guid utilisateurId, ActionSecurisee action, CancellationToken cancellationToken = default)
        => Task.FromResult(autorise);
}

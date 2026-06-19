using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;

namespace AppelOffreFreelance.Tests.Fakes;

public class FakePropositionRepository : IPropositionRepository
{
    private readonly List<PropositionFreelance> _propositions = [];

    public Task AjouterAsync(PropositionFreelance proposition, CancellationToken cancellationToken = default)
    {
        _propositions.Add(proposition);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<PropositionFreelance>> ObtenirParAppelOffreAsync(Guid appelOffreId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<PropositionFreelance>>(_propositions.Where(p => p.AppelOffreId == appelOffreId).ToList());
}

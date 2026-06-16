using CVTech.Modules.AppelOffreFreelance.Domain.Entites;

namespace CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;

public interface IPropositionRepository
{
    Task AjouterAsync(PropositionFreelance proposition, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropositionFreelance>> ObtenirParAppelOffreAsync(Guid appelOffreId, CancellationToken cancellationToken = default);
}

using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.AppelOffreFreelance.Infrastructure.Repositories;

public class PropositionRepository(AppelOffreDbContext db) : IPropositionRepository
{
    public async Task AjouterAsync(PropositionFreelance proposition, CancellationToken cancellationToken = default)
    {
        db.Propositions.Add(proposition);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PropositionFreelance>> ObtenirParAppelOffreAsync(Guid appelOffreId, CancellationToken cancellationToken = default)
        => await db.Propositions.Where(p => p.AppelOffreId == appelOffreId).ToListAsync(cancellationToken);
}

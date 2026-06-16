using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.CatalogueEmploi.Infrastructure.Repositories;

public class CandidatureRepository(CatalogueEmploiDbContext db) : ICandidatureRepository
{
    public async Task AjouterAsync(Candidature candidature, CancellationToken cancellationToken = default)
    {
        db.Candidatures.Add(candidature);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExisteDejaAsync(Guid candidatId, Guid annonceId, CancellationToken cancellationToken = default)
        => await db.Candidatures.AnyAsync(c => c.CandidatId == candidatId && c.AnnonceId == annonceId, cancellationToken);

    public async Task<IReadOnlyList<Candidature>> ObtenirParAnnonceAsync(Guid annonceId, CancellationToken cancellationToken = default)
        => await db.Candidatures.Where(c => c.AnnonceId == annonceId).ToListAsync(cancellationToken);
}

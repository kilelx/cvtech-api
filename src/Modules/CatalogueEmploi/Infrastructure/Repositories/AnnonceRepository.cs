using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.CatalogueEmploi.Infrastructure.Repositories;

public class AnnonceRepository(CatalogueEmploiDbContext db) : IAnnonceRepository
{
    public async Task<AnnonceEmploi?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await db.Annonces.FindAsync([id], cancellationToken);

    public async Task AjouterAsync(AnnonceEmploi annonce, CancellationToken cancellationToken = default)
    {
        db.Annonces.Add(annonce);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AnnonceEmploi>> ListerActivesAsync(CancellationToken cancellationToken = default)
        => await db.Annonces.Where(a => a.EstActive).ToListAsync(cancellationToken);
}

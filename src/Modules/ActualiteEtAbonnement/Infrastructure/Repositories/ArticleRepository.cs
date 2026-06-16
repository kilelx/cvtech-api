using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;

public class ArticleRepository(ActualiteDbContext db) : IArticleRepository
{
    public async Task AjouterAsync(ArticleActualite article, CancellationToken cancellationToken = default)
    {
        db.Articles.Add(article);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ArticleActualite>> ListerAsync(string? domaineMetier = null, CancellationToken cancellationToken = default)
    {
        var query = db.Articles.AsQueryable();
        if (domaineMetier is not null)
            query = query.Where(a => a.DomaineMetier == domaineMetier);
        return await query.OrderByDescending(a => a.DatePublication).ToListAsync(cancellationToken);
    }
}

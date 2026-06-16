using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;

public class AbonnementRepository(ActualiteDbContext db) : IAbonnementRepository
{
    public async Task AjouterAsync(Abonnement abonnement, CancellationToken cancellationToken = default)
    {
        db.Abonnements.Add(abonnement);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExisteDejaAsync(Guid utilisateurId, string domaineMetier, CancellationToken cancellationToken = default)
        => await db.Abonnements.AnyAsync(a => a.UtilisateurId == utilisateurId && a.DomaineMetier == domaineMetier, cancellationToken);

    public async Task<IReadOnlyList<Abonnement>> ObtenirParDomaineAsync(string domaineMetier, CancellationToken cancellationToken = default)
        => await db.Abonnements.Where(a => a.DomaineMetier == domaineMetier).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Abonnement>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
        => await db.Abonnements.Where(a => a.UtilisateurId == utilisateurId).ToListAsync(cancellationToken);

    public async Task SupprimerAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var abonnement = await db.Abonnements
            .FirstOrDefaultAsync(a => a.Id == id && a.UtilisateurId == utilisateurId, cancellationToken)
            ?? throw new AbonnementNonTrouveException(id);
        db.Abonnements.Remove(abonnement);
        await db.SaveChangesAsync(cancellationToken);
    }
}

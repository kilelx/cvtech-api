using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;

public class NotificationRepository(ActualiteDbContext db) : INotificationRepository
{
    public async Task AjouterAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        db.Notifications.Add(notification);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
        => await db.Notifications
            .Where(n => n.UtilisateurId == utilisateurId)
            .OrderByDescending(n => n.DateEnvoi)
            .ToListAsync(cancellationToken);

    public async Task MarquerCommeLueAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var notif = await db.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UtilisateurId == utilisateurId, cancellationToken)
            ?? throw new NotificationNonTrouveException(id);
        notif.MarquerCommeLue();
        await db.SaveChangesAsync(cancellationToken);
    }
}

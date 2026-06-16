using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface INotificationRepository
{
    Task AjouterAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default);
    Task MarquerCommeLueAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default);
}

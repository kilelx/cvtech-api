using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;

public class ServiceNotificationDb(
    INotificationRepository notificationRepository,
    ILogger<ServiceNotificationDb> logger) : IServiceNotification
{
    public async Task EnvoyerAsync(Abonnement abonnement, string titre, string corps, CancellationToken cancellationToken = default)
    {
        var notification = Notification.Creer(abonnement.UtilisateurId, titre, corps, abonnement.Canal);
        await notificationRepository.AjouterAsync(notification, cancellationToken);

        if (abonnement.Canal == CanalDiffusion.Email)
            logger.LogInformation("[EMAIL SIMULÉ] → {UserId} : {Titre}", abonnement.UtilisateurId, titre);
    }
}

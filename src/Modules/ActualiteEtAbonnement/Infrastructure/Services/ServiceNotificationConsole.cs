using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;

public class ServiceNotificationConsole(ILogger<ServiceNotificationConsole> logger) : IServiceNotification
{
    public Task EnvoyerAsync(Abonnement abonnement, string titre, string corps, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION][{Canal}] → Utilisateur {UserId} : {Titre} — {Corps}",
            abonnement.Canal, abonnement.UtilisateurId, titre, corps);
        return Task.CompletedTask;
    }
}

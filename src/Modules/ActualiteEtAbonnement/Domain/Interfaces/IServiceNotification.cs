using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface IServiceNotification
{
    Task EnvoyerAsync(Abonnement abonnement, string titre, string corps, CancellationToken cancellationToken = default);
}

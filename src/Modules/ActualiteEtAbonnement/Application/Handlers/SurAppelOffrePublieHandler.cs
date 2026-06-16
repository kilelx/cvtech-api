using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Contracts.Events;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Handlers;

public class SurAppelOffrePublieHandler(
    IAbonnementRepository abonnementRepository,
    IServiceNotification serviceNotification
) : INotificationHandler<AppelOffrePublie>
{
    public async Task Handle(AppelOffrePublie notification, CancellationToken cancellationToken)
    {
        var abonnes = await abonnementRepository.ObtenirParDomaineAsync(notification.DomaineMetier, cancellationToken);
        foreach (var abonne in abonnes)
        {
            await serviceNotification.EnvoyerAsync(
                abonne,
                $"Nouvel appel d'offre dans le domaine {notification.DomaineMetier}",
                $"Un nouvel appel d'offre freelance vient d'être publié.",
                cancellationToken);
        }
    }
}

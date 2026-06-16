using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Contracts.Events;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Handlers;

public class SurAnnoncePublieeHandler(
    IAbonnementRepository abonnementRepository,
    IServiceNotification serviceNotification
) : INotificationHandler<AnnoncePubliee>
{
    public async Task Handle(AnnoncePubliee notification, CancellationToken cancellationToken)
    {
        var abonnes = await abonnementRepository.ObtenirParDomaineAsync(notification.DomaineMetier, cancellationToken);
        foreach (var abonne in abonnes)
        {
            await serviceNotification.EnvoyerAsync(
                abonne,
                $"Nouvelle annonce dans le domaine {notification.DomaineMetier}",
                $"Une nouvelle annonce d'emploi vient d'être publiée.",
                cancellationToken);
        }
    }
}

using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirNotifications;

public class ObtenirNotificationsQueryHandler(INotificationRepository repository)
    : IRequestHandler<ObtenirNotificationsQuery, IReadOnlyList<Notification>>
{
    public Task<IReadOnlyList<Notification>> Handle(ObtenirNotificationsQuery request, CancellationToken cancellationToken)
        => repository.ObtenirParUtilisateurAsync(request.UtilisateurId, cancellationToken);
}

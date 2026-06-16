using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.MarquerNotificationLue;

public class MarquerNotificationLueCommandHandler(INotificationRepository repository)
    : IRequestHandler<MarquerNotificationLueCommand>
{
    public Task Handle(MarquerNotificationLueCommand request, CancellationToken cancellationToken)
        => repository.MarquerCommeLueAsync(request.NotificationId, request.UtilisateurId, cancellationToken);
}

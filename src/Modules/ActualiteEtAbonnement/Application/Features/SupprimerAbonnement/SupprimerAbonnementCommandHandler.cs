using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;

public class SupprimerAbonnementCommandHandler(IAbonnementRepository repository)
    : IRequestHandler<SupprimerAbonnementCommand>
{
    public Task Handle(SupprimerAbonnementCommand request, CancellationToken cancellationToken)
        => repository.SupprimerAsync(request.AbonnementId, request.UtilisateurId, cancellationToken);
}

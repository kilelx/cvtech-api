using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirAbonnements;

public class ObtenirAbonnementsQueryHandler(IAbonnementRepository repository)
    : IRequestHandler<ObtenirAbonnementsQuery, IReadOnlyList<Abonnement>>
{
    public Task<IReadOnlyList<Abonnement>> Handle(ObtenirAbonnementsQuery request, CancellationToken cancellationToken)
        => repository.ObtenirParUtilisateurAsync(request.UtilisateurId, cancellationToken);
}

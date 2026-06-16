using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ListerAnnonces;

public class ListerAnnoncesQueryHandler(IAnnonceRepository repository)
    : IRequestHandler<ListerAnnoncesQuery, IReadOnlyList<AnnonceEmploi>>
{
    public Task<IReadOnlyList<AnnonceEmploi>> Handle(ListerAnnoncesQuery request, CancellationToken cancellationToken)
        => repository.ListerActivesAsync(cancellationToken);
}

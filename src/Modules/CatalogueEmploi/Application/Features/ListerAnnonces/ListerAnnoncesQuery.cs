using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ListerAnnonces;

public record ListerAnnoncesQuery : IRequest<IReadOnlyList<AnnonceEmploi>>;

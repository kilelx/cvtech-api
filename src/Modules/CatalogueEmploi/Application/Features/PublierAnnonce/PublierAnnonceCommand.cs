using CVTech.Modules.CatalogueEmploi.Domain.Enums;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;

public record PublierAnnonceCommand(
    Guid UtilisateurId,
    string Titre,
    string Description,
    string DomaineMetier,
    TypeContrat TypeContrat
) : IRequest<Guid>;

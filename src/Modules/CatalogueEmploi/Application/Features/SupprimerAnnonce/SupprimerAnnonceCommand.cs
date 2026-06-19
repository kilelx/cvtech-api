using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.SupprimerAnnonce;

public record SupprimerAnnonceCommand(
    Guid UtilisateurId,
    Guid AnnonceId
) : IRequest;

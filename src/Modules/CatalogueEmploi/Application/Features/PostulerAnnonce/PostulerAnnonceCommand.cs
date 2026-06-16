using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;

public record PostulerAnnonceCommand(
    Guid UtilisateurId,
    Guid AnnonceId,
    Guid CurriculumVitaeId,
    string? LettreMotivation
) : IRequest<Guid>;

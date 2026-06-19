using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SupprimerAppelOffre;

public record SupprimerAppelOffreCommand(
    Guid UtilisateurId,
    Guid AppelOffreId
) : IRequest;

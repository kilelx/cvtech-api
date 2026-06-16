using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;

public record AbonnerDomaineCommand(
    Guid UtilisateurId,
    string DomaineMetier,
    CanalDiffusion Canal
) : IRequest;

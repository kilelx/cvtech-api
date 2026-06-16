using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;

public record SupprimerAbonnementCommand(Guid AbonnementId, Guid UtilisateurId) : IRequest;

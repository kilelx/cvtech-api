using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirAbonnements;

public record ObtenirAbonnementsQuery(Guid UtilisateurId) : IRequest<IReadOnlyList<Abonnement>>;

using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirNotifications;

public record ObtenirNotificationsQuery(Guid UtilisateurId) : IRequest<IReadOnlyList<Notification>>;

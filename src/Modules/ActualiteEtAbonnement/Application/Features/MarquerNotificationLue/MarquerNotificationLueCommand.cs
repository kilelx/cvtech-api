using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.MarquerNotificationLue;

public record MarquerNotificationLueCommand(Guid NotificationId, Guid UtilisateurId) : IRequest;

using CVTech.Modules.ActualiteEtAbonnement.Application.Features.MarquerNotificationLue;
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirNotifications;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notifs = await sender.Send(new ObtenirNotificationsQuery(uid), ct);
        return Ok(notifs.Select(n => new NotificationDto(n.Id, n.Titre, n.Corps, n.Canal.ToString(), n.EstLue, n.DateEnvoi)));
    }

    [HttpPatch("{id:guid}/lue")]
    public async Task<IActionResult> MarquerLue(Guid id, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new MarquerNotificationLueCommand(id, uid), ct);
        return NoContent();
    }
}

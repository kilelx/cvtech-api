using CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/abonnements")]
public class AbonnementController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Abonner([FromBody] AbonnerDomaineRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new AbonnerDomaineCommand(uid, dto.DomaineMetier, dto.Canal), ct);
        return NoContent();
    }
}

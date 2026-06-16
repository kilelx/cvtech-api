using CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.ObtenirAbonnements;
using CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/abonnements")]
public class AbonnementController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var abonnements = await sender.Send(new ObtenirAbonnementsQuery(uid), ct);
        return Ok(abonnements.Select(a => new AbonnementDto(a.Id, a.DomaineMetier, a.Canal.ToString(), a.DateAbonnement)));
    }

    [HttpPost]
    public async Task<IActionResult> Abonner([FromBody] AbonnerDomaineRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new AbonnerDomaineCommand(uid, dto.DomaineMetier, dto.Canal), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Supprimer(Guid id, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await sender.Send(new SupprimerAbonnementCommand(id, uid), ct);
        return NoContent();
    }
}

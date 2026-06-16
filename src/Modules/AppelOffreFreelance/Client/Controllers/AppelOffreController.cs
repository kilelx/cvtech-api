using CVTech.Modules.AppelOffreFreelance.Application.Features.ListerAppelsOffre;
using CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;
using CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;
using CVTech.Modules.AppelOffreFreelance.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.AppelOffreFreelance.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/appels-offre")]
public class AppelOffreController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var appels = await sender.Send(new ListerAppelsOffreQuery(), ct);
        return Ok(appels.Select(a => new
        {
            a.Id,
            a.Titre,
            a.Contexte,
            a.DomaineMetier,
            a.BudgetMax,
            a.Deadline,
            a.EntrepriseId,
            a.DatePublication,
            Statut = a.Statut.ToString(),
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierAppelOffreRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierAppelOffreCommand(uid, dto.Titre, dto.Contexte, dto.DomaineMetier, dto.BudgetMax, dto.Deadline), ct);
        return CreatedAtAction(nameof(Publier), new { id }, new { id });
    }

    [HttpPost("{appelOffreId:guid}/propositions")]
    public async Task<IActionResult> SoumettreProposition(Guid appelOffreId, [FromBody] SoumettrePropositionRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new SoumettrePropositionCommand(uid, appelOffreId, dto.TauxJournalierMoyen, dto.DureeEstimeeJours, dto.Methodologie), ct);
        return CreatedAtAction(nameof(SoumettreProposition), new { id }, new { id });
    }
}

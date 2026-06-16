using CVTech.Modules.CatalogueEmploi.Application.Features.ListerAnnonces;
using CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;
using CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;
using CVTech.Modules.CatalogueEmploi.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.CatalogueEmploi.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/annonces")]
public class AnnonceController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Lister(CancellationToken ct)
    {
        var annonces = await sender.Send(new ListerAnnoncesQuery(), ct);
        return Ok(annonces.Select(a => new
        {
            a.Id,
            a.Titre,
            a.Description,
            a.DomaineMetier,
            TypeContrat = a.TypeContrat.ToString(),
            a.EntrepriseId,
            a.DatePublication,
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierAnnonceCommand(utilisateurId, dto.Titre, dto.Description, dto.DomaineMetier, dto.TypeContrat), ct);
        return CreatedAtAction(nameof(Publier), new { id }, new { id });
    }

    [HttpPost("{annonceId:guid}/candidatures")]
    public async Task<IActionResult> Postuler(Guid annonceId, [FromBody] PostulerAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PostulerAnnonceCommand(utilisateurId, annonceId, dto.CurriculumVitaeId, dto.LettreMotivation), ct);
        return CreatedAtAction(nameof(Postuler), new { id }, new { id });
    }
}

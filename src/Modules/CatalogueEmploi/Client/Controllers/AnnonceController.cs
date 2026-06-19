using CVTech.Modules.CatalogueEmploi.Application.Features.ListerAnnonces;
using CVTech.Modules.CatalogueEmploi.Application.Features.SupprimerAnnonce;
using CVTech.Modules.CatalogueEmploi.Application.Features.ObtenirAnnoncesEntreprise;
using CVTech.Modules.CatalogueEmploi.Application.Features.ObtenirCandidatures;
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

    [HttpGet("mes-annonces")]
    public async Task<IActionResult> MesAnnonces(CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("Claim NameIdentifier manquant."));
        var annonces = await sender.Send(new ObtenirAnnoncesEntrepriseQuery(utilisateurId), ct);
        return Ok(annonces.Select(a => new
        {
            a.Id,
            a.Titre,
            a.Description,
            a.DomaineMetier,
            TypeContrat = a.TypeContrat.ToString(),
            a.DatePublication,
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("Claim NameIdentifier manquant."));
        var id = await sender.Send(new PublierAnnonceCommand(utilisateurId, dto.Titre, dto.Description, dto.DomaineMetier, dto.TypeContrat), ct);
        return CreatedAtAction(nameof(Publier), new { id }, new { id });
    }

    [HttpGet("{annonceId:guid}/candidatures")]
    public async Task<IActionResult> ObtenirCandidatures(Guid annonceId, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("Claim NameIdentifier manquant."));
        var candidatures = await sender.Send(new ObtenirCandidaturesQuery(utilisateurId, annonceId), ct);
        return Ok(candidatures.Select(c => new
        {
            c.Id,
            c.CandidatId,
            c.AnnonceId,
            c.CurriculumVitaeId,
            c.LettreMotivation,
            c.DateDepot,
        }));
    }

    [HttpDelete("{annonceId:guid}")]
    public async Task<IActionResult> Supprimer(Guid annonceId, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("Claim NameIdentifier manquant."));
        await sender.Send(new SupprimerAnnonceCommand(utilisateurId, annonceId), ct);
        return NoContent();
    }

    [HttpPost("{annonceId:guid}/candidatures")]
    public async Task<IActionResult> Postuler(Guid annonceId, [FromBody] PostulerAnnonceRequest dto, CancellationToken ct)
    {
        var utilisateurId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("Claim NameIdentifier manquant."));
        var id = await sender.Send(new PostulerAnnonceCommand(utilisateurId, annonceId, dto.CurriculumVitaeId, dto.LettreMotivation), ct);
        return CreatedAtAction(nameof(Postuler), new { id }, new { id });
    }
}

using CVTech.Modules.ActualiteEtAbonnement.Application.Features.PublierArticle;
using CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Rss;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;

[ApiController]
[Authorize]
[Route("api/actualites")]
public class ActualiteController(ISender sender, IArticleRepository articleRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Publier([FromBody] PublierArticleRequest dto, CancellationToken ct)
    {
        var uid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var id = await sender.Send(new PublierArticleCommand(uid, dto.Titre, dto.Contenu, dto.DomaineMetier), ct);
        return CreatedAtAction(nameof(Publier), new { id }, null);
    }

    [AllowAnonymous]
    [HttpGet("/feed/rss")]
    public async Task<IActionResult> Rss([FromQuery] string? domaine, CancellationToken ct)
    {
        var articles = await articleRepository.ListerAsync(domaine, ct);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var xml = GenerateurRss.GenererFlux(articles, baseUrl);
        return Content(xml, "application/rss+xml; charset=utf-8");
    }
}

using CVTech.Modules.GestionIdentite.Application.Features.ConnecterUtilisateur;
using CVTech.Modules.GestionIdentite.Application.Features.InscrireCandidat;
using CVTech.Modules.GestionIdentite.Application.Features.InscrireEntreprise;
using CVTech.Modules.GestionIdentite.Client.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CVTech.Modules.GestionIdentite.Client.Controllers;

[ApiController]
[Route("api/identite")]
public class IdentiteController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentiteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("candidats/inscription")]
    public async Task<IActionResult> InscrireCandidat([FromBody] InscrireCandidatRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new InscrireCandidatCommand(request.Prenom, request.Nom, request.Email, request.MotDePasse), cancellationToken);
        return CreatedAtAction(nameof(InscrireCandidat), new { id }, new { id });
    }

    [HttpPost("entreprises/inscription")]
    public async Task<IActionResult> InscrireEntreprise([FromBody] InscrireEntrepriseRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new InscrireEntrepriseCommand(request.RaisonSociale, request.Siret, request.Email, request.MotDePasse), cancellationToken);
        return CreatedAtAction(nameof(InscrireEntreprise), new { id }, new { id });
    }

    [HttpPost("connexion")]
    public async Task<IActionResult> Connecter([FromBody] ConnexionRequest request, CancellationToken cancellationToken)
    {
        var resultat = await _mediator.Send(new ConnecterUtilisateurQuery(request.Email, request.MotDePasse), cancellationToken);
        return Ok(resultat);
    }
}

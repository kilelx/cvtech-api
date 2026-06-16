using MediatR;

namespace CVTech.Modules.GestionIdentite.Application.Features.InscrireEntreprise;

public record InscrireEntrepriseCommand(
    string RaisonSociale,
    string Siret,
    string Email,
    string MotDePasse
) : IRequest<Guid>;

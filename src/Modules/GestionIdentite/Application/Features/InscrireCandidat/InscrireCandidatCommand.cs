using MediatR;

namespace CVTech.Modules.GestionIdentite.Application.Features.InscrireCandidat;

public record InscrireCandidatCommand(
    string Prenom,
    string Nom,
    string Email,
    string MotDePasse
) : IRequest<Guid>;

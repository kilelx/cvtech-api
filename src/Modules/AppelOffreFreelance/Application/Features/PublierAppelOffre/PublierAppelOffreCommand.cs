using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;

public record PublierAppelOffreCommand(
    Guid UtilisateurId,
    string Titre,
    string Contexte,
    string DomaineMetier,
    decimal BudgetMax,
    DateTime Deadline
) : IRequest<Guid>;

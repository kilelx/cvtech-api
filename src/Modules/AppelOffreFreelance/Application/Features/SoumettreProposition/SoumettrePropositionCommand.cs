using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;

public record SoumettrePropositionCommand(
    Guid UtilisateurId,
    Guid AppelOffreId,
    decimal TauxJournalierMoyen,
    int DureeEstimeeJours,
    string Methodologie
) : IRequest<Guid>;

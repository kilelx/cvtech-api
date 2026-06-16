using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;
using CVTech.Modules.AppelOffreFreelance.Domain.Exceptions;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SoumettreProposition;

public class SoumettrePropositionCommandHandler(
    IAppelOffreRepository appelOffreRepository,
    IPropositionRepository propositionRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<SoumettrePropositionCommand, Guid>
{
    public async Task<Guid> Handle(SoumettrePropositionCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.SoumettrePropositionFreelance, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : soumission de proposition.");

        var ao = await appelOffreRepository.ObtenirParIdAsync(request.AppelOffreId, cancellationToken)
            ?? throw new AppelOffreIntrouvableException(request.AppelOffreId);

        if (ao.Statut == StatutAppelOffre.Clos)
            throw new AppelOffreClosException(ao.Id);

        var proposition = PropositionFreelance.Creer(ao.Id, request.UtilisateurId, request.TauxJournalierMoyen, request.DureeEstimeeJours, request.Methodologie);
        await propositionRepository.AjouterAsync(proposition, cancellationToken);
        return proposition.Id;
    }
}

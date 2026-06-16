using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Exceptions;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PostulerAnnonce;

public class PostulerAnnonceCommandHandler(
    IAnnonceRepository annonceRepository,
    ICandidatureRepository candidatureRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<PostulerAnnonceCommand, Guid>
{
    public async Task<Guid> Handle(PostulerAnnonceCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PostulerAnnonce, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : candidature.");

        var annonce = await annonceRepository.ObtenirParIdAsync(request.AnnonceId, cancellationToken)
            ?? throw new AnnonceIntrouvableException(request.AnnonceId);

        if (await candidatureRepository.ExisteDejaAsync(request.UtilisateurId, annonce.Id, cancellationToken))
            throw new CandidatureDejaExistanteException(request.UtilisateurId, annonce.Id);

        var candidature = Candidature.Creer(request.UtilisateurId, annonce.Id, request.CurriculumVitaeId, request.LettreMotivation);
        await candidatureRepository.AjouterAsync(candidature, cancellationToken);
        return candidature.Id;
    }
}

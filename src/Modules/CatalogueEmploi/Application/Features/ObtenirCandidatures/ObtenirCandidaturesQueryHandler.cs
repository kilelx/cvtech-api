using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Exceptions;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ObtenirCandidatures;

public class ObtenirCandidaturesQueryHandler(
    IAnnonceRepository annonceRepository,
    ICandidatureRepository candidatureRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<ObtenirCandidaturesQuery, IReadOnlyList<Candidature>>
{
    public async Task<IReadOnlyList<Candidature>> Handle(ObtenirCandidaturesQuery request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.ConsulterCandidaturesRecues, cancellationToken))
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ConsulterCandidaturesRecues));

        var annonce = await annonceRepository.ObtenirParIdAsync(request.AnnonceId, cancellationToken)
            ?? throw new AnnonceIntrouvableException(request.AnnonceId);

        if (annonce.EntrepriseId != request.UtilisateurId)
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ConsulterCandidaturesRecues));

        return await candidatureRepository.ObtenirParAnnonceAsync(request.AnnonceId, cancellationToken);
    }
}

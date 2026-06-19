using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ObtenirAnnoncesEntreprise;

public class ObtenirAnnoncesEntrepriseQueryHandler(
    IAnnonceRepository annonceRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<ObtenirAnnoncesEntrepriseQuery, IReadOnlyList<AnnonceEmploi>>
{
    public async Task<IReadOnlyList<AnnonceEmploi>> Handle(ObtenirAnnoncesEntrepriseQuery request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierAnnonceEmploi, cancellationToken))
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.PublierAnnonceEmploi));

        return await annonceRepository.ListerParEntrepriseAsync(request.UtilisateurId, cancellationToken);
    }
}

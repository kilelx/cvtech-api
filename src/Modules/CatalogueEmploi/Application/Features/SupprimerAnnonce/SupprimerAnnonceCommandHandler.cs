using CVTech.Modules.CatalogueEmploi.Domain.Exceptions;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.SupprimerAnnonce;

public class SupprimerAnnonceCommandHandler(
    IAnnonceRepository annonceRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<SupprimerAnnonceCommand>
{
    public async Task Handle(SupprimerAnnonceCommand request, CancellationToken cancellationToken)
    {
        var annonce = await annonceRepository.ObtenirParIdAsync(request.AnnonceId, cancellationToken)
            ?? throw new AnnonceIntrouvableException(request.AnnonceId);

        var estAdmin = await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.ModererContenu, cancellationToken);
        var estProprietaire = annonce.EntrepriseId == request.UtilisateurId
            && await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierAnnonceEmploi, cancellationToken);

        if (!estAdmin && !estProprietaire)
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ModererContenu));

        await annonceRepository.SupprimerAsync(request.AnnonceId, cancellationToken);
    }
}

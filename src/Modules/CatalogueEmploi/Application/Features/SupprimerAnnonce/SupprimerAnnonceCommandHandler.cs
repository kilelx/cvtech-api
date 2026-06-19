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
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.ModererContenu, cancellationToken))
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ModererContenu));

        await annonceRepository.SupprimerAsync(request.AnnonceId, cancellationToken);
    }
}

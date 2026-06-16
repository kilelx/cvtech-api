using CVTech.Modules.CatalogueEmploi.Contracts.Events;
using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.PublierAnnonce;

public class PublierAnnonceCommandHandler(
    IAnnonceRepository annonceRepository,
    IVerificateurPermission verificateur,
    IPublisher publisher
) : IRequestHandler<PublierAnnonceCommand, Guid>
{
    public async Task<Guid> Handle(PublierAnnonceCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierAnnonceEmploi, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : publication d'annonce.");

        var annonce = AnnonceEmploi.Creer(request.Titre, request.Description, request.DomaineMetier, request.TypeContrat, request.UtilisateurId);
        await annonceRepository.AjouterAsync(annonce, cancellationToken);
        await publisher.Publish(new AnnoncePubliee(annonce.Id, annonce.DomaineMetier, annonce.EntrepriseId), cancellationToken);
        return annonce.Id;
    }
}

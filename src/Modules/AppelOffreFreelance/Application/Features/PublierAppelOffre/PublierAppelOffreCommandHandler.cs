using CVTech.Modules.AppelOffreFreelance.Contracts.Events;
using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.PublierAppelOffre;

public class PublierAppelOffreCommandHandler(
    IAppelOffreRepository repository,
    IVerificateurPermission verificateur,
    IPublisher publisher
) : IRequestHandler<PublierAppelOffreCommand, Guid>
{
    public async Task<Guid> Handle(PublierAppelOffreCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierAppelOffre, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : publication d'appel d'offre.");

        var ao = AppelOffre.Creer(request.Titre, request.Contexte, request.DomaineMetier, request.BudgetMax, request.Deadline, request.UtilisateurId);
        await repository.AjouterAsync(ao, cancellationToken);
        await publisher.Publish(new AppelOffrePublie(ao.Id, ao.DomaineMetier, ao.EntrepriseId), cancellationToken);
        return ao.Id;
    }
}

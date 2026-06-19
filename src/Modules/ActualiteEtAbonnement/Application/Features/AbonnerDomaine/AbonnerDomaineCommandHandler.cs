using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.AbonnerDomaine;

public class AbonnerDomaineCommandHandler(
    IAbonnementRepository abonnementRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<AbonnerDomaineCommand>
{
    public async Task Handle(AbonnerDomaineCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.SAbonnerDomaineMetier, cancellationToken))
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.SAbonnerDomaineMetier));

        if (await abonnementRepository.ExisteDejaAsync(request.UtilisateurId, request.DomaineMetier, cancellationToken))
            throw new AbonnementDejaExistantException(request.UtilisateurId, request.DomaineMetier);

        var abonnement = Abonnement.Creer(request.UtilisateurId, request.DomaineMetier, request.Canal);
        await abonnementRepository.AjouterAsync(abonnement, cancellationToken);
    }
}

using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.SupprimerAppelOffre;

public class SupprimerAppelOffreCommandHandler(
    IAppelOffreRepository repository,
    IVerificateurPermission verificateur
) : IRequestHandler<SupprimerAppelOffreCommand>
{
    public async Task Handle(SupprimerAppelOffreCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.ModererContenu, cancellationToken))
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ModererContenu));

        await repository.SupprimerAsync(request.AppelOffreId, cancellationToken);
    }
}

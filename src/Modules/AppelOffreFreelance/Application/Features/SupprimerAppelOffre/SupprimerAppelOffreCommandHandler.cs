using CVTech.Modules.AppelOffreFreelance.Domain.Exceptions;
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
        var ao = await repository.ObtenirParIdAsync(request.AppelOffreId, cancellationToken)
            ?? throw new AppelOffreIntrouvableException(request.AppelOffreId);

        var estAdmin = await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.ModererContenu, cancellationToken);
        var estProprietaire = ao.EntrepriseId == request.UtilisateurId
            && await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierAppelOffre, cancellationToken);

        if (!estAdmin && !estProprietaire)
            throw new PermissionRefuseeException(request.UtilisateurId, nameof(ActionSecurisee.ModererContenu));

        await repository.SupprimerAsync(request.AppelOffreId, cancellationToken);
    }
}

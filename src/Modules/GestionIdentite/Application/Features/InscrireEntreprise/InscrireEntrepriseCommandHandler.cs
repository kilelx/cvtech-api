using CVTech.Modules.GestionIdentite.Domain.Entites;
using CVTech.Modules.GestionIdentite.Domain.Exceptions;
using CVTech.Modules.GestionIdentite.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.GestionIdentite.Application.Features.InscrireEntreprise;

public class InscrireEntrepriseCommandHandler : IRequestHandler<InscrireEntrepriseCommand, Guid>
{
    private readonly IProfilRepository _profilRepository;

    public InscrireEntrepriseCommandHandler(IProfilRepository profilRepository)
    {
        _profilRepository = profilRepository;
    }

    public async Task<Guid> Handle(InscrireEntrepriseCommand request, CancellationToken cancellationToken)
    {
        var existant = await _profilRepository.ObtenirEntrepriseParEmailAsync(request.Email, cancellationToken);
        if (existant is not null)
            throw new UtilisateurExistantException(request.Email);

        var motDePasseHache = BCrypt.Net.BCrypt.HashPassword(request.MotDePasse);
        var profil = ProfilEntreprise.Creer(request.RaisonSociale, request.Siret, request.Email, motDePasseHache);

        await _profilRepository.AjouterEntrepriseAsync(profil, cancellationToken);
        await _profilRepository.SauvegarderAsync(cancellationToken);

        return profil.Id;
    }
}

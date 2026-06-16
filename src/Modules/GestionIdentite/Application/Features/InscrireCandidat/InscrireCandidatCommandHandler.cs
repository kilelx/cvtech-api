using CVTech.Modules.GestionIdentite.Domain.Entites;
using CVTech.Modules.GestionIdentite.Domain.Exceptions;
using CVTech.Modules.GestionIdentite.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.GestionIdentite.Application.Features.InscrireCandidat;

public class InscrireCandidatCommandHandler : IRequestHandler<InscrireCandidatCommand, Guid>
{
    private readonly IProfilRepository _profilRepository;

    public InscrireCandidatCommandHandler(IProfilRepository profilRepository)
    {
        _profilRepository = profilRepository;
    }

    public async Task<Guid> Handle(InscrireCandidatCommand request, CancellationToken cancellationToken)
    {
        var existant = await _profilRepository.ObtenirCandidatParEmailAsync(request.Email, cancellationToken);
        if (existant is not null)
            throw new UtilisateurExistantException(request.Email);

        var motDePasseHache = BCrypt.Net.BCrypt.HashPassword(request.MotDePasse);
        var profil = ProfilCandidat.Creer(request.Prenom, request.Nom, request.Email, motDePasseHache);

        await _profilRepository.AjouterCandidatAsync(profil, cancellationToken);
        await _profilRepository.SauvegarderAsync(cancellationToken);

        return profil.Id;
    }
}

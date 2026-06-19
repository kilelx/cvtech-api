using CVTech.Modules.GestionIdentite.Domain.Interfaces;
using MediatR;


namespace CVTech.Modules.GestionIdentite.Application.Features.ConnecterUtilisateur;

public class ConnecterUtilisateurQueryHandler : IRequestHandler<ConnecterUtilisateurQuery, ConnexionResultat>
{
    private readonly IProfilRepository _profilRepository;
    private readonly IJwtService _jwtService;

    public ConnecterUtilisateurQueryHandler(IProfilRepository profilRepository, IJwtService jwtService)
    {
        _profilRepository = profilRepository;
        _jwtService = jwtService;
    }

    public async Task<ConnexionResultat> Handle(ConnecterUtilisateurQuery request, CancellationToken cancellationToken)
    {
        var candidat = await _profilRepository.ObtenirCandidatParEmailAsync(request.Email, cancellationToken);
        if (candidat is not null && BCrypt.Net.BCrypt.Verify(request.MotDePasse, candidat.MotDePasseHache))
        {
            var token = _jwtService.GenererToken(candidat.Id, candidat.Role.ToString());
            return new ConnexionResultat(candidat.Id, candidat.Role.ToString(), token);
        }

        var entreprise = await _profilRepository.ObtenirEntrepriseParEmailAsync(request.Email, cancellationToken);
        if (entreprise is not null && BCrypt.Net.BCrypt.Verify(request.MotDePasse, entreprise.MotDePasseHache))
        {
            var token = _jwtService.GenererToken(entreprise.Id, entreprise.Role.ToString());
            return new ConnexionResultat(entreprise.Id, entreprise.Role.ToString(), token);
        }

        var admin = await _profilRepository.ObtenirAdministrateurParEmailAsync(request.Email, cancellationToken);
        if (admin is not null && BCrypt.Net.BCrypt.Verify(request.MotDePasse, admin.MotDePasseHache))
        {
            var token = _jwtService.GenererToken(admin.Id, admin.Role.ToString());
            return new ConnexionResultat(admin.Id, admin.Role.ToString(), token);
        }

        throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");
    }
}

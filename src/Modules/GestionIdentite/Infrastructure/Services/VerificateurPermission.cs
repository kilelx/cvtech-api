using CVTech.Modules.GestionIdentite.Contracts;
using CVTech.Modules.GestionIdentite.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Domain.Services;

namespace CVTech.Modules.GestionIdentite.Infrastructure.Services;

public class VerificateurPermission : IVerificateurPermission
{
    private readonly IProfilRepository _profilRepository;

    public VerificateurPermission(IProfilRepository profilRepository)
    {
        _profilRepository = profilRepository;
    }

    public async Task<bool> AutoriserAsync(Guid utilisateurId, ActionSecurisee action, CancellationToken cancellationToken = default)
    {
        var role = await _profilRepository.ObtenirRoleAsync(utilisateurId, cancellationToken);
        if (role is null) return false;

        var estActif = await _profilRepository.EstCompteActifAsync(utilisateurId, cancellationToken);
        if (!estActif) return false;

        return MatricePermissions.EstAutorise(role.Value, action);
    }
}

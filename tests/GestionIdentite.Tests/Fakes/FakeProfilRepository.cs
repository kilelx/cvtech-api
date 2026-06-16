using CVTech.Modules.GestionIdentite.Domain.Entites;
using CVTech.Modules.GestionIdentite.Domain.Enums;
using CVTech.Modules.GestionIdentite.Domain.Interfaces;

namespace GestionIdentite.Tests.Fakes;

public class FakeProfilRepository : IProfilRepository
{
    private readonly List<ProfilCandidat> _candidats = [];
    private readonly List<ProfilEntreprise> _entreprises = [];
    private readonly List<Administrateur> _admins = [];

    public void AjouterCandidat(ProfilCandidat candidat) => _candidats.Add(candidat);
    public void AjouterEntreprise(ProfilEntreprise entreprise) => _entreprises.Add(entreprise);
    public void AjouterAdmin(Administrateur admin) => _admins.Add(admin);

    public Task<RoleUtilisateur?> ObtenirRoleAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var candidat = _candidats.FirstOrDefault(c => c.Id == utilisateurId);
        if (candidat is not null) return Task.FromResult<RoleUtilisateur?>(candidat.Role);

        var entreprise = _entreprises.FirstOrDefault(e => e.Id == utilisateurId);
        if (entreprise is not null) return Task.FromResult<RoleUtilisateur?>(entreprise.Role);

        var admin = _admins.FirstOrDefault(a => a.Id == utilisateurId);
        if (admin is not null) return Task.FromResult<RoleUtilisateur?>(admin.Role);

        return Task.FromResult<RoleUtilisateur?>(null);
    }

    public Task<bool> EstCompteActifAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var candidat = _candidats.FirstOrDefault(c => c.Id == utilisateurId);
        if (candidat is not null) return Task.FromResult(candidat.EstActif);

        var entreprise = _entreprises.FirstOrDefault(e => e.Id == utilisateurId);
        if (entreprise is not null) return Task.FromResult(entreprise.EstActif);

        var admin = _admins.FirstOrDefault(a => a.Id == utilisateurId);
        return Task.FromResult(admin is not null);
    }

    public Task<ProfilCandidat?> ObtenirCandidatParEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(_candidats.FirstOrDefault(c => c.Email == email));

    public Task<ProfilEntreprise?> ObtenirEntrepriseParEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(_entreprises.FirstOrDefault(e => e.Email == email));

    public Task AjouterCandidatAsync(ProfilCandidat profil, CancellationToken cancellationToken = default)
    {
        _candidats.Add(profil);
        return Task.CompletedTask;
    }

    public Task AjouterEntrepriseAsync(ProfilEntreprise profil, CancellationToken cancellationToken = default)
    {
        _entreprises.Add(profil);
        return Task.CompletedTask;
    }

    public Task SauvegarderAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

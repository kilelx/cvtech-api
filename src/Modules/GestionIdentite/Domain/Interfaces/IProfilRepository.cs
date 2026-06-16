using CVTech.Modules.GestionIdentite.Domain.Entites;
using CVTech.Modules.GestionIdentite.Domain.Enums;

namespace CVTech.Modules.GestionIdentite.Domain.Interfaces;

public interface IProfilRepository
{
    Task<RoleUtilisateur?> ObtenirRoleAsync(Guid utilisateurId, CancellationToken cancellationToken = default);
    Task<ProfilCandidat?> ObtenirCandidatParEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ProfilEntreprise?> ObtenirEntrepriseParEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AjouterCandidatAsync(ProfilCandidat profil, CancellationToken cancellationToken = default);
    Task AjouterEntrepriseAsync(ProfilEntreprise profil, CancellationToken cancellationToken = default);
    Task<bool> EstCompteActifAsync(Guid utilisateurId, CancellationToken cancellationToken = default);
    Task SauvegarderAsync(CancellationToken cancellationToken = default);
}

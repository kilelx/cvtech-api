using CVTech.Modules.GestionIdentite.Domain.Entites;
using CVTech.Modules.GestionIdentite.Domain.Enums;
using CVTech.Modules.GestionIdentite.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.GestionIdentite.Infrastructure.Repositories;

public class ProfilRepository : IProfilRepository
{
    private readonly GestionIdentiteDbContext _context;

    public ProfilRepository(GestionIdentiteDbContext context)
    {
        _context = context;
    }

    public async Task<RoleUtilisateur?> ObtenirRoleAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var candidat = await _context.ProfilsCandidats.FindAsync([utilisateurId], cancellationToken);
        if (candidat is not null) return candidat.Role;

        var entreprise = await _context.ProfilsEntreprises.FindAsync([utilisateurId], cancellationToken);
        if (entreprise is not null) return entreprise.Role;

        var admin = await _context.Administrateurs.FindAsync([utilisateurId], cancellationToken);
        if (admin is not null) return admin.Role;

        return null;
    }

    public Task<ProfilCandidat?> ObtenirCandidatParEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.ProfilsCandidats.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<ProfilEntreprise?> ObtenirEntrepriseParEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.ProfilsEntreprises.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task AjouterCandidatAsync(ProfilCandidat profil, CancellationToken cancellationToken = default)
        => await _context.ProfilsCandidats.AddAsync(profil, cancellationToken);

    public async Task AjouterEntrepriseAsync(ProfilEntreprise profil, CancellationToken cancellationToken = default)
        => await _context.ProfilsEntreprises.AddAsync(profil, cancellationToken);

    public async Task<bool> EstCompteActifAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
    {
        var candidat = await _context.ProfilsCandidats.FindAsync([utilisateurId], cancellationToken);
        if (candidat is not null) return candidat.EstActif;

        var entreprise = await _context.ProfilsEntreprises.FindAsync([utilisateurId], cancellationToken);
        if (entreprise is not null) return entreprise.EstActif;

        var admin = await _context.Administrateurs.FindAsync([utilisateurId], cancellationToken);
        return admin is not null;
    }

    public Task SauvegarderAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

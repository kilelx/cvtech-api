using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Enums;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.AppelOffreFreelance.Infrastructure.Repositories;

public class AppelOffreRepository(AppelOffreDbContext db) : IAppelOffreRepository
{
    public async Task<AppelOffre?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await db.AppelsOffre.FindAsync([id], cancellationToken);

    public async Task AjouterAsync(AppelOffre appelOffre, CancellationToken cancellationToken = default)
    {
        db.AppelsOffre.Add(appelOffre);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppelOffre>> ListerOuvertsAsync(CancellationToken cancellationToken = default)
        => await db.AppelsOffre.Where(a => a.Statut == StatutAppelOffre.Ouvert).ToListAsync(cancellationToken);
}

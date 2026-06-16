using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;

public class CatalogueEmploiDbContext(DbContextOptions<CatalogueEmploiDbContext> options) : DbContext(options)
{
    public DbSet<AnnonceEmploi> Annonces => Set<AnnonceEmploi>();
    public DbSet<CurriculumVitae> CurriculumsVitae => Set<CurriculumVitae>();
    public DbSet<Candidature> Candidatures => Set<Candidature>();
}

using CVTech.Modules.GestionIdentite.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.GestionIdentite.Infrastructure.Persistence;

public class GestionIdentiteDbContext : DbContext
{
    public GestionIdentiteDbContext(DbContextOptions<GestionIdentiteDbContext> options) : base(options) { }

    public DbSet<ProfilCandidat> ProfilsCandidats => Set<ProfilCandidat>();
    public DbSet<ProfilEntreprise> ProfilsEntreprises => Set<ProfilEntreprise>();
    public DbSet<Administrateur> Administrateurs => Set<Administrateur>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gestion_identite");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestionIdentiteDbContext).Assembly,
            t => t.Namespace!.Contains("GestionIdentite.Infrastructure.Persistence.Configurations"));
    }
}

using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;

public class AppelOffreDbContext(DbContextOptions<AppelOffreDbContext> options) : DbContext(options)
{
    public DbSet<AppelOffre> AppelsOffre => Set<AppelOffre>();
    public DbSet<PropositionFreelance> Propositions => Set<PropositionFreelance>();
}

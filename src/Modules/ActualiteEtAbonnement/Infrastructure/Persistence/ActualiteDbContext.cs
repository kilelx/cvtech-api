using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;

public class ActualiteDbContext(DbContextOptions<ActualiteDbContext> options) : DbContext(options)
{
    public DbSet<ArticleActualite> Articles => Set<ArticleActualite>();
    public DbSet<Abonnement> Abonnements => Set<Abonnement>();
    public DbSet<Notification> Notifications => Set<Notification>();
}

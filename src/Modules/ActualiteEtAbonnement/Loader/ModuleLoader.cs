using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Repositories;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVTech.Modules.ActualiteEtAbonnement.Loader;

public static class ModuleLoader
{
    public static IServiceCollection AjouterModuleActualiteEtAbonnement(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? configureDb = null)
    {
        services.AddDbContext<ActualiteDbContext>(options =>
        {
            if (configureDb != null) configureDb(options);
            else options.UseSqlServer(configuration.GetConnectionString("ActualiteEtAbonnement"));
        });

        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IAbonnementRepository, AbonnementRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IServiceNotification, ServiceNotificationDb>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}

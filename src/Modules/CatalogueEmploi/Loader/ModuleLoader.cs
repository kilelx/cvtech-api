using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVTech.Modules.CatalogueEmploi.Loader;

public static class ModuleLoader
{
    public static IServiceCollection AjouterModuleCatalogueEmploi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? configureDb = null)
    {
        services.AddDbContext<CatalogueEmploiDbContext>(options =>
        {
            if (configureDb != null) configureDb(options);
            else options.UseSqlServer(configuration.GetConnectionString("CatalogueEmploi"));
        });

        services.AddScoped<IAnnonceRepository, AnnonceRepository>();
        services.AddScoped<ICandidatureRepository, CandidatureRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}

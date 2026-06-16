using CVTech.Modules.GestionIdentite.Contracts;
using CVTech.Modules.GestionIdentite.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Infrastructure.Persistence;
using CVTech.Modules.GestionIdentite.Infrastructure.Repositories;
using CVTech.Modules.GestionIdentite.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVTech.Modules.GestionIdentite.Loader;

public static class ModuleLoader
{
    public static IServiceCollection AjouterModuleGestionIdentite(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? configureDb = null)
    {
        services.AddDbContext<GestionIdentiteDbContext>(options =>
        {
            if (configureDb != null) configureDb(options);
            else options.UseSqlServer(configuration.GetConnectionString("GestionIdentite"));
        });

        services.AddScoped<IProfilRepository, ProfilRepository>();
        services.AddScoped<IVerificateurPermission, VerificateurPermission>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}

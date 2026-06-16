using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVTech.Modules.AppelOffreFreelance.Loader;

public static class ModuleLoader
{
    public static IServiceCollection AjouterModuleAppelOffreFreelance(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? configureDb = null)
    {
        services.AddDbContext<AppelOffreDbContext>(options =>
        {
            if (configureDb != null) configureDb(options);
            else options.UseSqlServer(configuration.GetConnectionString("AppelOffreFreelance"));
        });

        services.AddScoped<IAppelOffreRepository, AppelOffreRepository>();
        services.AddScoped<IPropositionRepository, PropositionRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ModuleLoader).Assembly));

        return services;
    }
}

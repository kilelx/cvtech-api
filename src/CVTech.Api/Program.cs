using System.Text;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Persistence;
using CVTech.Modules.ActualiteEtAbonnement.Client.Controllers;
using CVTech.Modules.ActualiteEtAbonnement.Loader;
using CVTech.Modules.AppelOffreFreelance.Client.Controllers;
using CVTech.Modules.AppelOffreFreelance.Infrastructure.Persistence;
using CVTech.Modules.AppelOffreFreelance.Loader;
using CVTech.Modules.CatalogueEmploi.Client.Controllers;
using CVTech.Modules.CatalogueEmploi.Infrastructure.Persistence;
using CVTech.Modules.CatalogueEmploi.Loader;
using CVTech.Modules.GestionIdentite.Client.Controllers;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.AppelOffreFreelance.Domain.Exceptions;
using CVTech.Modules.CatalogueEmploi.Domain.Exceptions;
using CVTech.Modules.GestionIdentite.Contracts;
using CVTech.Modules.GestionIdentite.Domain.Exceptions;
using CVTech.Modules.GestionIdentite.Infrastructure.Persistence;
using CVTech.Modules.GestionIdentite.Loader;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

// ── Modules ──────────────────────────────────────────────────────────────────
Action<DbContextOptionsBuilder>? dbConfig = env.IsDevelopment()
    ? null  // individual SQLite paths set per-module below
    : null; // prod uses SQL Server via connection strings in config

if (env.IsDevelopment())
{
    builder.Services.AjouterModuleGestionIdentite(config,
        o => o.UseSqlite("Data Source=gestion_identite.db"));
    builder.Services.AjouterModuleCatalogueEmploi(config,
        o => o.UseSqlite("Data Source=catalogue_emploi.db"));
    builder.Services.AjouterModuleAppelOffreFreelance(config,
        o => o.UseSqlite("Data Source=appel_offre.db"));
    builder.Services.AjouterModuleActualiteEtAbonnement(config,
        o => o.UseSqlite("Data Source=actualite.db"));
}
else
{
    builder.Services.AjouterModuleGestionIdentite(config);
    builder.Services.AjouterModuleCatalogueEmploi(config);
    builder.Services.AjouterModuleAppelOffreFreelance(config);
    builder.Services.AjouterModuleActualiteEtAbonnement(config);
}

// ── Authentication / JWT ─────────────────────────────────────────────────────
var jwtSecret = config["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret manquant dans la configuration.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (env.IsDevelopment())
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else
            policy
                .WithOrigins(config.GetSection("Cors:Origins").Get<string[]>() ?? [])
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

// ── Controllers (découverte dans chaque assembly module) ──────────────────────
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()))
    .AddApplicationPart(typeof(IdentiteController).Assembly)
    .AddApplicationPart(typeof(AnnonceController).Assembly)
    .AddApplicationPart(typeof(AppelOffreController).Assembly)
    .AddApplicationPart(typeof(ActualiteController).Assembly);

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CVTech API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            []
        }
    });
});

// ── Pipeline ──────────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;
    sp.GetRequiredService<GestionIdentiteDbContext>().Database.Migrate();
    sp.GetRequiredService<CatalogueEmploiDbContext>().Database.Migrate();
    sp.GetRequiredService<AppelOffreDbContext>().Database.Migrate();
    sp.GetRequiredService<ActualiteDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp => errorApp.Run(async ctx =>
{
    var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var error = feature?.Error;
    ctx.Response.ContentType = "application/json";
    (int statusCode, string message) = error switch
    {
        PermissionRefuseeException _ => (403, "Permission refusée."),
        UnauthorizedAccessException e => (401, e.Message),
        AnnonceIntrouvableException e => (404, e.Message),
        AppelOffreIntrouvableException e => (404, e.Message),
        ArticleIntrouvableException e => (404, e.Message),
        AbonnementNonTrouveException e => (404, e.Message),
        NotificationNonTrouveException e => (404, e.Message),
        UtilisateurExistantException e => (409, e.Message),
        CandidatureDejaExistanteException e => (409, e.Message),
        AbonnementDejaExistantException e => (409, e.Message),
        AppelOffreClosException e => (409, e.Message),
        _ => (500, "Une erreur interne est survenue.")
    };
    ctx.Response.StatusCode = statusCode;
    await ctx.Response.WriteAsJsonAsync(new { error = message });
}));

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

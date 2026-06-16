namespace CVTech.Modules.GestionIdentite.Domain.Interfaces;

public interface IJwtService
{
    string GenererToken(Guid utilisateurId, string role);
}

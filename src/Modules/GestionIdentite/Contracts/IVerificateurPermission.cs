namespace CVTech.Modules.GestionIdentite.Contracts;

public interface IVerificateurPermission
{
    Task<bool> AutoriserAsync(Guid utilisateurId, ActionSecurisee action, CancellationToken cancellationToken = default);
}

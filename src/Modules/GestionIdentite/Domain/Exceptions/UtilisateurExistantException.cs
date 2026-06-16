namespace CVTech.Modules.GestionIdentite.Domain.Exceptions;

public class UtilisateurExistantException : Exception
{
    public UtilisateurExistantException(string email)
        : base($"Un utilisateur avec l'email '{email}' existe déjà.") { }
}

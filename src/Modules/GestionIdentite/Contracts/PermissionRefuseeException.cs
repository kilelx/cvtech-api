namespace CVTech.Modules.GestionIdentite.Contracts;

public class PermissionRefuseeException : Exception
{
    public Guid UtilisateurId { get; }
    public string Action { get; }

    public PermissionRefuseeException(Guid utilisateurId, string action)
        : base($"L'utilisateur {utilisateurId} n'est pas autorisé à effectuer l'action '{action}'.")
    {
        UtilisateurId = utilisateurId;
        Action = action;
    }
}

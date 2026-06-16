using CVTech.Modules.GestionIdentite.Domain.Enums;

namespace CVTech.Modules.GestionIdentite.Domain.Entites;

public class Administrateur
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string MotDePasseHache { get; private set; } = string.Empty;
    public RoleUtilisateur Role { get; private set; } = RoleUtilisateur.Administrateur;
    public DateTime DateCreation { get; private set; }

    private Administrateur() { }

    public static Administrateur Creer(string email, string motDePasseHache)
    {
        return new Administrateur
        {
            Id = Guid.NewGuid(),
            Email = email,
            MotDePasseHache = motDePasseHache,
            DateCreation = DateTime.UtcNow,
        };
    }
}

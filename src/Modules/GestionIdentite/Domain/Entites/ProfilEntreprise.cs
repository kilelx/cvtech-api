using CVTech.Modules.GestionIdentite.Domain.Enums;

namespace CVTech.Modules.GestionIdentite.Domain.Entites;

public class ProfilEntreprise
{
    public Guid Id { get; private set; }
    public string RaisonSociale { get; private set; } = string.Empty;
    public string Siret { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string MotDePasseHache { get; private set; } = string.Empty;
    public RoleUtilisateur Role { get; private set; } = RoleUtilisateur.Entreprise;
    public bool EstActif { get; private set; } = true;
    public DateTime DateInscription { get; private set; }

    private ProfilEntreprise() { }

    public static ProfilEntreprise Creer(string raisonSociale, string siret, string email, string motDePasseHache)
    {
        return new ProfilEntreprise
        {
            Id = Guid.NewGuid(),
            RaisonSociale = raisonSociale,
            Siret = siret,
            Email = email,
            MotDePasseHache = motDePasseHache,
            DateInscription = DateTime.UtcNow,
        };
    }

    public void Bloquer() => EstActif = false;
    public void Reactiver() => EstActif = true;
}

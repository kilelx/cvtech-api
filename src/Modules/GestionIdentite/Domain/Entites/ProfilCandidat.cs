using CVTech.Modules.GestionIdentite.Domain.Enums;

namespace CVTech.Modules.GestionIdentite.Domain.Entites;

public class ProfilCandidat
{
    public Guid Id { get; private set; }
    public string Prenom { get; private set; } = string.Empty;
    public string Nom { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string MotDePasseHache { get; private set; } = string.Empty;
    public RoleUtilisateur Role { get; private set; } = RoleUtilisateur.Candidat;
    public bool EstActif { get; private set; } = true;
    public Guid? CurriculumVitaeId { get; private set; }
    public DateTime DateInscription { get; private set; }

    private ProfilCandidat() { }

    public static ProfilCandidat Creer(string prenom, string nom, string email, string motDePasseHache)
    {
        return new ProfilCandidat
        {
            Id = Guid.NewGuid(),
            Prenom = prenom,
            Nom = nom,
            Email = email,
            MotDePasseHache = motDePasseHache,
            DateInscription = DateTime.UtcNow,
        };
    }

    public void Bloquer() => EstActif = false;
    public void Reactiver() => EstActif = true;
    public void AssocierCV(Guid cvId) => CurriculumVitaeId = cvId;
}

using CVTech.Modules.CatalogueEmploi.Domain.Enums;

namespace CVTech.Modules.CatalogueEmploi.Domain.Entites;

public class AnnonceEmploi
{
    public Guid Id { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string DomaineMetier { get; private set; } = string.Empty;
    public TypeContrat TypeContrat { get; private set; }
    public Guid EntrepriseId { get; private set; }
    public bool EstActive { get; private set; } = true;
    public DateTime DatePublication { get; private set; }

    private AnnonceEmploi() { }

    public static AnnonceEmploi Creer(string titre, string description, string domaineMetier, TypeContrat typeContrat, Guid entrepriseId)
    {
        return new AnnonceEmploi
        {
            Id = Guid.NewGuid(),
            Titre = titre,
            Description = description,
            DomaineMetier = domaineMetier,
            TypeContrat = typeContrat,
            EntrepriseId = entrepriseId,
            DatePublication = DateTime.UtcNow,
        };
    }

    public void Desactiver() => EstActive = false;
}

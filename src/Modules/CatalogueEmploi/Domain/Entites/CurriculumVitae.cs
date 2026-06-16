namespace CVTech.Modules.CatalogueEmploi.Domain.Entites;

public class CurriculumVitae
{
    public Guid Id { get; private set; }
    public Guid CandidatId { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Resume { get; private set; } = string.Empty;
    public DateTime DerniereModification { get; private set; }

    private CurriculumVitae() { }

    public static CurriculumVitae Creer(Guid candidatId, string titre, string resume)
    {
        return new CurriculumVitae
        {
            Id = Guid.NewGuid(),
            CandidatId = candidatId,
            Titre = titre,
            Resume = resume,
            DerniereModification = DateTime.UtcNow,
        };
    }

    public void Modifier(string titre, string resume)
    {
        Titre = titre;
        Resume = resume;
        DerniereModification = DateTime.UtcNow;
    }
}

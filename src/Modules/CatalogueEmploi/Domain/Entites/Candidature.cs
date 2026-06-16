namespace CVTech.Modules.CatalogueEmploi.Domain.Entites;

public class Candidature
{
    public Guid Id { get; private set; }
    public Guid CandidatId { get; private set; }
    public Guid AnnonceId { get; private set; }
    public Guid CurriculumVitaeId { get; private set; }
    public string? LettreMotivation { get; private set; }
    public DateTime DateDepot { get; private set; }

    private Candidature() { }

    public static Candidature Creer(Guid candidatId, Guid annonceId, Guid curriculumVitaeId, string? lettreMotivation)
    {
        return new Candidature
        {
            Id = Guid.NewGuid(),
            CandidatId = candidatId,
            AnnonceId = annonceId,
            CurriculumVitaeId = curriculumVitaeId,
            LettreMotivation = lettreMotivation,
            DateDepot = DateTime.UtcNow,
        };
    }
}

namespace CVTech.Modules.AppelOffreFreelance.Domain.Entites;

public class PropositionFreelance
{
    public Guid Id { get; private set; }
    public Guid AppelOffreId { get; private set; }
    public Guid CandidatId { get; private set; }
    public decimal TauxJournalierMoyen { get; private set; }
    public int DureeEstimeeJours { get; private set; }
    public string Methodologie { get; private set; } = string.Empty;
    public DateTime DateSoumission { get; private set; }

    private PropositionFreelance() { }

    public static PropositionFreelance Creer(Guid appelOffreId, Guid candidatId, decimal tjm, int duree, string methodologie)
    {
        return new PropositionFreelance
        {
            Id = Guid.NewGuid(),
            AppelOffreId = appelOffreId,
            CandidatId = candidatId,
            TauxJournalierMoyen = tjm,
            DureeEstimeeJours = duree,
            Methodologie = methodologie,
            DateSoumission = DateTime.UtcNow,
        };
    }
}

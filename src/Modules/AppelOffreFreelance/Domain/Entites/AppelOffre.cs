using CVTech.Modules.AppelOffreFreelance.Domain.Enums;

namespace CVTech.Modules.AppelOffreFreelance.Domain.Entites;

public class AppelOffre
{
    public Guid Id { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Contexte { get; private set; } = string.Empty;
    public string DomaineMetier { get; private set; } = string.Empty;
    public decimal BudgetMax { get; private set; }
    public DateTime Deadline { get; private set; }
    public Guid EntrepriseId { get; private set; }
    public StatutAppelOffre Statut { get; private set; } = StatutAppelOffre.Ouvert;
    public DateTime DatePublication { get; private set; }

    private AppelOffre() { }

    public static AppelOffre Creer(string titre, string contexte, string domaineMetier, decimal budgetMax, DateTime deadline, Guid entrepriseId)
    {
        return new AppelOffre
        {
            Id = Guid.NewGuid(),
            Titre = titre,
            Contexte = contexte,
            DomaineMetier = domaineMetier,
            BudgetMax = budgetMax,
            Deadline = deadline,
            EntrepriseId = entrepriseId,
            DatePublication = DateTime.UtcNow,
        };
    }

    public void Clore() => Statut = StatutAppelOffre.Clos;
}

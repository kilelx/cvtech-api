using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

public class Abonnement
{
    public Guid Id { get; private set; }
    public Guid UtilisateurId { get; private set; }
    public string DomaineMetier { get; private set; } = string.Empty;
    public CanalDiffusion Canal { get; private set; }
    public DateTime DateAbonnement { get; private set; }

    private Abonnement() { }

    public static Abonnement Creer(Guid utilisateurId, string domaineMetier, CanalDiffusion canal)
    {
        return new Abonnement
        {
            Id = Guid.NewGuid(),
            UtilisateurId = utilisateurId,
            DomaineMetier = domaineMetier,
            Canal = canal,
            DateAbonnement = DateTime.UtcNow,
        };
    }
}

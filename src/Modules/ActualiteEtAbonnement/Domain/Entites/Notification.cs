using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid UtilisateurId { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Corps { get; private set; } = string.Empty;
    public CanalDiffusion Canal { get; private set; }
    public bool EstLue { get; private set; }
    public DateTime DateEnvoi { get; private set; }

    private Notification() { }

    public static Notification Creer(Guid utilisateurId, string titre, string corps, CanalDiffusion canal)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UtilisateurId = utilisateurId,
            Titre = titre,
            Corps = corps,
            Canal = canal,
            DateEnvoi = DateTime.UtcNow,
        };
    }

    public void MarquerCommeLue() => EstLue = true;
}

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

public class ArticleActualite
{
    public Guid Id { get; private set; }
    public string Titre { get; private set; } = string.Empty;
    public string Contenu { get; private set; } = string.Empty;
    public string? DomaineMetier { get; private set; }
    public Guid AuteurId { get; private set; }
    public DateTime DatePublication { get; private set; }

    private ArticleActualite() { }

    public static ArticleActualite Creer(string titre, string contenu, Guid auteurId, string? domaineMetier = null)
    {
        return new ArticleActualite
        {
            Id = Guid.NewGuid(),
            Titre = titre,
            Contenu = contenu,
            DomaineMetier = domaineMetier,
            AuteurId = auteurId,
            DatePublication = DateTime.UtcNow,
        };
    }
}

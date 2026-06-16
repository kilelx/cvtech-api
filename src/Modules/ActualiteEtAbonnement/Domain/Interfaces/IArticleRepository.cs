using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface IArticleRepository
{
    Task AjouterAsync(ArticleActualite article, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArticleActualite>> ListerAsync(string? domaineMetier = null, CancellationToken cancellationToken = default);
}

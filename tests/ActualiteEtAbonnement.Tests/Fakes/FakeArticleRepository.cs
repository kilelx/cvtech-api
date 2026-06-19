using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

namespace ActualiteEtAbonnement.Tests.Fakes;

public class FakeArticleRepository : IArticleRepository
{
    private readonly List<ArticleActualite> _articles = [];

    public Task AjouterAsync(ArticleActualite article, CancellationToken cancellationToken = default)
    {
        _articles.Add(article);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ArticleActualite>> ListerAsync(string? domaineMetier = null, CancellationToken cancellationToken = default)
    {
        var result = domaineMetier is null
            ? _articles
            : _articles.Where(a => a.DomaineMetier == domaineMetier).ToList();
        return Task.FromResult<IReadOnlyList<ArticleActualite>>(result);
    }
}

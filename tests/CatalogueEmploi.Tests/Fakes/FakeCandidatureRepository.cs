using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;

namespace CatalogueEmploi.Tests.Fakes;

public class FakeCandidatureRepository : ICandidatureRepository
{
    private readonly List<Candidature> _candidatures = [];

    public Task AjouterAsync(Candidature candidature, CancellationToken cancellationToken = default)
    {
        _candidatures.Add(candidature);
        return Task.CompletedTask;
    }

    public Task<bool> ExisteDejaAsync(Guid candidatId, Guid annonceId, CancellationToken cancellationToken = default)
        => Task.FromResult(_candidatures.Any(c => c.CandidatId == candidatId && c.AnnonceId == annonceId));

    public Task<IReadOnlyList<Candidature>> ObtenirParAnnonceAsync(Guid annonceId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Candidature>>(_candidatures.Where(c => c.AnnonceId == annonceId).ToList());
}

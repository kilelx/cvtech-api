using CVTech.Modules.CatalogueEmploi.Domain.Entites;

namespace CVTech.Modules.CatalogueEmploi.Domain.Interfaces;

public interface ICandidatureRepository
{
    Task AjouterAsync(Candidature candidature, CancellationToken cancellationToken = default);
    Task<bool> ExisteDejaAsync(Guid candidatId, Guid annonceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Candidature>> ObtenirParAnnonceAsync(Guid annonceId, CancellationToken cancellationToken = default);
}

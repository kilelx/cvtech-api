using CVTech.Modules.CatalogueEmploi.Domain.Entites;

namespace CVTech.Modules.CatalogueEmploi.Domain.Interfaces;

public interface IAnnonceRepository
{
    Task<AnnonceEmploi?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AjouterAsync(AnnonceEmploi annonce, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnnonceEmploi>> ListerActivesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnnonceEmploi>> ListerParEntrepriseAsync(Guid entrepriseId, CancellationToken cancellationToken = default);
    Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default);
}

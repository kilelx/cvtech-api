using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using CVTech.Modules.CatalogueEmploi.Domain.Interfaces;

namespace CatalogueEmploi.Tests.Fakes;

public class FakeAnnonceRepository : IAnnonceRepository
{
    private readonly List<AnnonceEmploi> _annonces = [];

    public void Ajouter(AnnonceEmploi annonce) => _annonces.Add(annonce);

    public Task<AnnonceEmploi?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_annonces.FirstOrDefault(a => a.Id == id));

    public Task AjouterAsync(AnnonceEmploi annonce, CancellationToken cancellationToken = default)
    {
        _annonces.Add(annonce);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AnnonceEmploi>> ListerActivesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AnnonceEmploi>>(_annonces.Where(a => a.EstActive).ToList());

    public Task<IReadOnlyList<AnnonceEmploi>> ListerParEntrepriseAsync(Guid entrepriseId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AnnonceEmploi>>(_annonces.Where(a => a.EntrepriseId == entrepriseId).ToList());

    public Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _annonces.FirstOrDefault(a => a.Id == id)?.Desactiver();
        return Task.CompletedTask;
    }
}

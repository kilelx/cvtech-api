using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;

namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;

public interface IAbonnementRepository
{
    Task AjouterAsync(Abonnement abonnement, CancellationToken cancellationToken = default);
    Task<bool> ExisteDejaAsync(Guid utilisateurId, string domaineMetier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Abonnement>> ObtenirParDomaineAsync(string domaineMetier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Abonnement>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default);
    Task SupprimerAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default);
}

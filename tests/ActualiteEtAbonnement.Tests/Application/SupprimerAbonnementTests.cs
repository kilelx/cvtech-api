using CVTech.Modules.ActualiteEtAbonnement.Application.Features.SupprimerAbonnement;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using FluentAssertions;

namespace ActualiteEtAbonnement.Tests.Application;

public class SupprimerAbonnementTests
{
    private sealed class FakeAbonnementRepository : IAbonnementRepository
    {
        private readonly List<Abonnement> _store = [];

        public Task AjouterAsync(Abonnement abonnement, CancellationToken cancellationToken = default)
        {
            _store.Add(abonnement);
            return Task.CompletedTask;
        }

        public Task<bool> ExisteDejaAsync(Guid utilisateurId, string domaineMetier, CancellationToken cancellationToken = default)
            => Task.FromResult(_store.Any(a => a.UtilisateurId == utilisateurId && a.DomaineMetier == domaineMetier));

        public Task<IReadOnlyList<Abonnement>> ObtenirParDomaineAsync(string domaineMetier, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Abonnement>>(_store.Where(a => a.DomaineMetier == domaineMetier).ToList());

        public Task<IReadOnlyList<Abonnement>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Abonnement>>(_store.Where(a => a.UtilisateurId == utilisateurId).ToList());

        public Task SupprimerAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
        {
            var abonnement = _store.FirstOrDefault(a => a.Id == id && a.UtilisateurId == utilisateurId)
                ?? throw new AbonnementNonTrouveException(id);
            _store.Remove(abonnement);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Supprimer_QuandAbonnementExisteEtAppartientAuUser_Supprime()
    {
        var repo = new FakeAbonnementRepository();
        var userId = Guid.NewGuid();
        var abonnement = Abonnement.Creer(userId, "cloud", CanalDiffusion.InApp);
        await repo.AjouterAsync(abonnement);

        var handler = new SupprimerAbonnementCommandHandler(repo);
        await handler.Handle(new SupprimerAbonnementCommand(abonnement.Id, userId), default);

        var restants = await repo.ObtenirParUtilisateurAsync(userId);
        restants.Should().BeEmpty();
    }

    [Fact]
    public async Task Supprimer_QuandAbonnementAppartientAAutreUser_LeveException()
    {
        var repo = new FakeAbonnementRepository();
        var userId = Guid.NewGuid();
        var autreUserId = Guid.NewGuid();
        var abonnement = Abonnement.Creer(userId, "cloud", CanalDiffusion.InApp);
        await repo.AjouterAsync(abonnement);

        var handler = new SupprimerAbonnementCommandHandler(repo);
        var act = () => handler.Handle(new SupprimerAbonnementCommand(abonnement.Id, autreUserId), default);

        await act.Should().ThrowAsync<AbonnementNonTrouveException>();
    }
}

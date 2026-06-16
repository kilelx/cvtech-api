using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.ActualiteEtAbonnement.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ActualiteEtAbonnement.Tests.Application;

public class ServiceNotificationDbTests
{
    private sealed class FakeNotificationRepository : INotificationRepository
    {
        public List<Notification> Saved { get; } = [];

        public Task AjouterAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            Saved.Add(notification);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Notification>> ObtenirParUtilisateurAsync(Guid utilisateurId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Notification>>(Saved.Where(n => n.UtilisateurId == utilisateurId).ToList());

        public Task MarquerCommeLueAsync(Guid id, Guid utilisateurId, CancellationToken cancellationToken = default)
        {
            var notif = Saved.FirstOrDefault(n => n.Id == id && n.UtilisateurId == utilisateurId);
            notif?.MarquerCommeLue();
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Envoyer_QuandCanal_InApp_PersisteLaNotification()
    {
        var repo = new FakeNotificationRepository();
        var service = new ServiceNotificationDb(repo, NullLogger<ServiceNotificationDb>.Instance);
        var abonnement = Abonnement.Creer(Guid.NewGuid(), "cloud", CanalDiffusion.InApp);

        await service.EnvoyerAsync(abonnement, "Titre", "Corps");

        repo.Saved.Should().HaveCount(1);
        repo.Saved[0].UtilisateurId.Should().Be(abonnement.UtilisateurId);
        repo.Saved[0].Canal.Should().Be(CanalDiffusion.InApp);
        repo.Saved[0].EstLue.Should().BeFalse();
    }

    [Fact]
    public async Task Envoyer_QuandCanal_Email_PersisteLaNotification()
    {
        var repo = new FakeNotificationRepository();
        var service = new ServiceNotificationDb(repo, NullLogger<ServiceNotificationDb>.Instance);
        var abonnement = Abonnement.Creer(Guid.NewGuid(), "cloud", CanalDiffusion.Email);

        await service.EnvoyerAsync(abonnement, "Titre", "Corps");

        repo.Saved.Should().HaveCount(1);
        repo.Saved[0].Canal.Should().Be(CanalDiffusion.Email);
    }
}

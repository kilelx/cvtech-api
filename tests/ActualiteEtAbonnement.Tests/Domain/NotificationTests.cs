using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;
using FluentAssertions;

namespace ActualiteEtAbonnement.Tests.Domain;

public class NotificationTests
{
    [Fact]
    public void UneNotification_QuandCreee_ALesBonnesValeurs()
    {
        var userId = Guid.NewGuid();
        var notif = Notification.Creer(userId, "Titre", "Corps", CanalDiffusion.InApp);

        notif.UtilisateurId.Should().Be(userId);
        notif.Titre.Should().Be("Titre");
        notif.Corps.Should().Be("Corps");
        notif.Canal.Should().Be(CanalDiffusion.InApp);
        notif.EstLue.Should().BeFalse();
    }

    [Fact]
    public void UneNotification_QuandMarqueeCommeLue_EstLueEstTrue()
    {
        var notif = Notification.Creer(Guid.NewGuid(), "T", "C", CanalDiffusion.InApp);
        notif.MarquerCommeLue();
        notif.EstLue.Should().BeTrue();
    }
}

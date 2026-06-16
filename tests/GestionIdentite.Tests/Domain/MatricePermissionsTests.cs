using CVTech.Modules.GestionIdentite.Contracts;
using CVTech.Modules.GestionIdentite.Domain.Enums;
using CVTech.Modules.GestionIdentite.Domain.Services;
using FluentAssertions;

namespace GestionIdentite.Tests.Domain;

public class MatricePermissionsTests
{
    // ── Candidat ────────────────────────────────────────────────────────────

    [Fact]
    public void UnCandidat_QuandIlConsulteUneAnnonce_AlorsLActionEstAutorisee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Candidat, ActionSecurisee.ConsulterAnnonce);

        // Assert
        resultat.Should().BeTrue();
    }

    [Fact]
    public void UnCandidat_QuandIlPostule_AlorsLActionEstAutorisee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Candidat, ActionSecurisee.PostulerAnnonce);

        // Assert
        resultat.Should().BeTrue();
    }

    [Fact]
    public void UnCandidat_QuandIlTenteDePublierUneAnnonce_AlorsLActionEstRefusee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Candidat, ActionSecurisee.PublierAnnonceEmploi);

        // Assert
        resultat.Should().BeFalse();
    }

    [Fact]
    public void UnCandidat_QuandIlTenteDeModererLeContenu_AlorsLActionEstRefusee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Candidat, ActionSecurisee.ModererContenu);

        // Assert
        resultat.Should().BeFalse();
    }

    [Fact]
    public void UnCandidat_QuandIlTenteDeBloquerUnCompte_AlorsLActionEstRefusee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Candidat, ActionSecurisee.BloquerCompte);

        // Assert
        resultat.Should().BeFalse();
    }

    // ── Entreprise ──────────────────────────────────────────────────────────

    [Fact]
    public void UneEntreprise_QuandEllePublieUneAnnonceEmploi_AlorsLActionEstAutorisee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Entreprise, ActionSecurisee.PublierAnnonceEmploi);

        // Assert
        resultat.Should().BeTrue();
    }

    [Fact]
    public void UneEntreprise_QuandEllePublieUnAppelOffre_AlorsLActionEstAutorisee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Entreprise, ActionSecurisee.PublierAppelOffre);

        // Assert
        resultat.Should().BeTrue();
    }

    [Fact]
    public void UneEntreprise_QuandEllePostuleAUneAnnonce_AlorsLActionEstRefusee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Entreprise, ActionSecurisee.PostulerAnnonce);

        // Assert
        resultat.Should().BeFalse();
    }

    [Fact]
    public void UneEntreprise_QuandElleModifieUnCV_AlorsLActionEstRefusee()
    {
        // Arrange / Act
        var resultat = MatricePermissions.EstAutorise(RoleUtilisateur.Entreprise, ActionSecurisee.ModifierCV);

        // Assert
        resultat.Should().BeFalse();
    }

    // ── Administrateur ──────────────────────────────────────────────────────

    [Fact]
    public void UnAdministrateur_PourTouteAction_AlorsLActionEstAutorisee()
    {
        // Arrange
        var toutesLesActions = Enum.GetValues<ActionSecurisee>();

        // Act / Assert
        foreach (var action in toutesLesActions)
        {
            MatricePermissions.EstAutorise(RoleUtilisateur.Administrateur, action)
                .Should().BeTrue($"l'administrateur doit pouvoir effectuer l'action {action}");
        }
    }
}

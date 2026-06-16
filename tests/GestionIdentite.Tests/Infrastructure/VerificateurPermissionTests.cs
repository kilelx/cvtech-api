using CVTech.Modules.GestionIdentite.Contracts;
using CVTech.Modules.GestionIdentite.Domain.Entites;
using CVTech.Modules.GestionIdentite.Infrastructure.Services;
using FluentAssertions;
using GestionIdentite.Tests.Fakes;

namespace GestionIdentite.Tests.Infrastructure;

public class VerificateurPermissionTests
{
    // ── Candidat actif ──────────────────────────────────────────────────────

    [Fact]
    public async Task UnCandidatActif_QuandIlPostule_AlorsLActionEstAutorisee()
    {
        // Arrange
        var candidat = ProfilCandidat.Creer("Jean", "Dupont", "jean@test.fr", "hash");
        var repo = new FakeProfilRepository();
        repo.AjouterCandidat(candidat);
        var verificateur = new VerificateurPermission(repo);

        // Act
        var resultat = await verificateur.AutoriserAsync(candidat.Id, ActionSecurisee.PostulerAnnonce);

        // Assert
        resultat.Should().BeTrue("un candidat actif peut postuler");
    }

    // ── Compte bloqué ───────────────────────────────────────────────────────
    // 🔴 RED : VerificateurPermission ne vérifie pas EstActif — ce test doit échouer

    [Fact]
    public async Task UnCandidatBloque_QuandIlTenteDePostuler_AlorsLActionEstRefusee()
    {
        // Arrange
        var candidat = ProfilCandidat.Creer("Jean", "Dupont", "jean@test.fr", "hash");
        candidat.Bloquer();
        var repo = new FakeProfilRepository();
        repo.AjouterCandidat(candidat);
        var verificateur = new VerificateurPermission(repo);

        // Act
        var resultat = await verificateur.AutoriserAsync(candidat.Id, ActionSecurisee.PostulerAnnonce);

        // Assert
        resultat.Should().BeFalse("un compte bloqué ne peut effectuer aucune action");
    }

    [Fact]
    public async Task UnCandidatBloque_QuandIlTenteDeConsulterUneAnnonce_AlorsLActionEstRefusee()
    {
        // Arrange
        var candidat = ProfilCandidat.Creer("Jean", "Dupont", "jean@test.fr", "hash");
        candidat.Bloquer();
        var repo = new FakeProfilRepository();
        repo.AjouterCandidat(candidat);
        var verificateur = new VerificateurPermission(repo);

        // Act
        var resultat = await verificateur.AutoriserAsync(candidat.Id, ActionSecurisee.ConsulterAnnonce);

        // Assert
        resultat.Should().BeFalse("un compte bloqué perd tous ses droits, y compris la consultation");
    }

    // ── Utilisateur inconnu ─────────────────────────────────────────────────

    [Fact]
    public async Task UnUtilisateurInconnu_PourNImporteQuelleAction_AlorsLActionEstRefusee()
    {
        // Arrange
        var repo = new FakeProfilRepository();
        var verificateur = new VerificateurPermission(repo);

        // Act
        var resultat = await verificateur.AutoriserAsync(Guid.NewGuid(), ActionSecurisee.PostulerAnnonce);

        // Assert
        resultat.Should().BeFalse("un utilisateur inconnu ne peut rien faire");
    }
}

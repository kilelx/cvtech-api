using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.PublierArticle;

public record PublierArticleCommand(
    Guid UtilisateurId,
    string Titre,
    string Contenu,
    string? DomaineMetier
) : IRequest<Guid>;

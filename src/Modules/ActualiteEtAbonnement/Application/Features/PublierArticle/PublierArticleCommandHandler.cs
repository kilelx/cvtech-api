using CVTech.Modules.ActualiteEtAbonnement.Domain.Entites;
using CVTech.Modules.ActualiteEtAbonnement.Domain.Interfaces;
using CVTech.Modules.GestionIdentite.Contracts;
using MediatR;

namespace CVTech.Modules.ActualiteEtAbonnement.Application.Features.PublierArticle;

public class PublierArticleCommandHandler(
    IArticleRepository articleRepository,
    IVerificateurPermission verificateur
) : IRequestHandler<PublierArticleCommand, Guid>
{
    public async Task<Guid> Handle(PublierArticleCommand request, CancellationToken cancellationToken)
    {
        if (!await verificateur.AutoriserAsync(request.UtilisateurId, ActionSecurisee.PublierArticleActualite, cancellationToken))
            throw new UnauthorizedAccessException("Permission refusée : publication d'article.");

        var article = ArticleActualite.Creer(request.Titre, request.Contenu, request.UtilisateurId, request.DomaineMetier);
        await articleRepository.AjouterAsync(article, cancellationToken);
        return article.Id;
    }
}

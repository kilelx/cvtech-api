using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ObtenirCandidatures;

public record ObtenirCandidaturesQuery(Guid UtilisateurId, Guid AnnonceId) : IRequest<IReadOnlyList<Candidature>>;

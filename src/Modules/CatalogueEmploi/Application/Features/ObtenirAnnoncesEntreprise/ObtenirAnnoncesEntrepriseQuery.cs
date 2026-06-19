using CVTech.Modules.CatalogueEmploi.Domain.Entites;
using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Application.Features.ObtenirAnnoncesEntreprise;

public record ObtenirAnnoncesEntrepriseQuery(Guid UtilisateurId) : IRequest<IReadOnlyList<AnnonceEmploi>>;

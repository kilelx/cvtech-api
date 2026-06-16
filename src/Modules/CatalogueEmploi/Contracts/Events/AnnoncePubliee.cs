using MediatR;

namespace CVTech.Modules.CatalogueEmploi.Contracts.Events;

public record AnnoncePubliee(Guid AnnonceId, string DomaineMetier, Guid EntrepriseId) : INotification;

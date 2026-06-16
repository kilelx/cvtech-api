using CVTech.Modules.CatalogueEmploi.Domain.Enums;

namespace CVTech.Modules.CatalogueEmploi.Client.DTOs;

public record PublierAnnonceRequest(string Titre, string Description, string DomaineMetier, TypeContrat TypeContrat);

namespace CVTech.Modules.CatalogueEmploi.Domain.Exceptions;

public class AnnonceIntrouvableException(Guid annonceId)
    : Exception($"Annonce {annonceId} introuvable.");

public class CandidatureDejaExistanteException(Guid candidatId, Guid annonceId)
    : Exception($"Le candidat {candidatId} a déjà postulé à l'annonce {annonceId}.");

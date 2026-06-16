namespace CVTech.Modules.AppelOffreFreelance.Domain.Exceptions;

public class AppelOffreIntrouvableException(Guid id) : Exception($"Appel d'offre {id} introuvable.");
public class AppelOffreClosException(Guid id) : Exception($"L'appel d'offre {id} est clos, les propositions ne sont plus acceptées.");

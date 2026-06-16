namespace CVTech.Modules.ActualiteEtAbonnement.Domain.Exceptions;

public class ArticleIntrouvableException(Guid id) : Exception($"Article {id} introuvable.");
public class AbonnementDejaExistantException(Guid utilisateurId, string domaine)
    : Exception($"L'utilisateur {utilisateurId} est déjà abonné au domaine '{domaine}'.");

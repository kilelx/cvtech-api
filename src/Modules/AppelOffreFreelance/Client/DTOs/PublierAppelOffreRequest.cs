namespace CVTech.Modules.AppelOffreFreelance.Client.DTOs;

public record PublierAppelOffreRequest(string Titre, string Contexte, string DomaineMetier, decimal BudgetMax, DateTime Deadline);

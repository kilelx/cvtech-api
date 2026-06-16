namespace CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;

public record NotificationDto(Guid Id, string Titre, string Corps, string Canal, bool EstLue, DateTime DateEnvoi);

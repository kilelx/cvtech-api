using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Contracts.Events;

public record AppelOffrePublie(Guid AppelOffreId, string DomaineMetier, Guid EntrepriseId) : INotification;

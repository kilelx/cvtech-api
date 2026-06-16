using CVTech.Modules.ActualiteEtAbonnement.Domain.Enums;

namespace CVTech.Modules.ActualiteEtAbonnement.Client.DTOs;

public record AbonnerDomaineRequest(string DomaineMetier, CanalDiffusion Canal);

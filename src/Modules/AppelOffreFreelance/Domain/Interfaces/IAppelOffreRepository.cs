using CVTech.Modules.AppelOffreFreelance.Domain.Entites;

namespace CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;

public interface IAppelOffreRepository
{
    Task<AppelOffre?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AjouterAsync(AppelOffre appelOffre, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppelOffre>> ListerOuvertsAsync(CancellationToken cancellationToken = default);
}

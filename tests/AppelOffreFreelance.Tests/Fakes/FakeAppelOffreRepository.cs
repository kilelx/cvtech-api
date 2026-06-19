using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;

namespace AppelOffreFreelance.Tests.Fakes;

public class FakeAppelOffreRepository : IAppelOffreRepository
{
    private readonly List<AppelOffre> _appelsOffre = [];

    public void Ajouter(AppelOffre ao) => _appelsOffre.Add(ao);

    public Task<AppelOffre?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_appelsOffre.FirstOrDefault(a => a.Id == id));

    public Task AjouterAsync(AppelOffre appelOffre, CancellationToken cancellationToken = default)
    {
        _appelsOffre.Add(appelOffre);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AppelOffre>> ListerOuvertsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AppelOffre>>(_appelsOffre.ToList());

    public Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _appelsOffre.FirstOrDefault(a => a.Id == id)?.Clore();
        return Task.CompletedTask;
    }
}

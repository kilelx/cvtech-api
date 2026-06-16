using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using CVTech.Modules.AppelOffreFreelance.Domain.Interfaces;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.ListerAppelsOffre;

public class ListerAppelsOffreQueryHandler(IAppelOffreRepository repository)
    : IRequestHandler<ListerAppelsOffreQuery, IReadOnlyList<AppelOffre>>
{
    public Task<IReadOnlyList<AppelOffre>> Handle(ListerAppelsOffreQuery request, CancellationToken cancellationToken)
        => repository.ListerOuvertsAsync(cancellationToken);
}

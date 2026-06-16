using CVTech.Modules.AppelOffreFreelance.Domain.Entites;
using MediatR;

namespace CVTech.Modules.AppelOffreFreelance.Application.Features.ListerAppelsOffre;

public record ListerAppelsOffreQuery : IRequest<IReadOnlyList<AppelOffre>>;

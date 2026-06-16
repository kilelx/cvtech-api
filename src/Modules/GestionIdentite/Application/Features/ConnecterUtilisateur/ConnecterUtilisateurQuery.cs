using MediatR;

namespace CVTech.Modules.GestionIdentite.Application.Features.ConnecterUtilisateur;

public record ConnecterUtilisateurQuery(string Email, string MotDePasse) : IRequest<ConnexionResultat>;

public record ConnexionResultat(Guid UtilisateurId, string Role, string Token);

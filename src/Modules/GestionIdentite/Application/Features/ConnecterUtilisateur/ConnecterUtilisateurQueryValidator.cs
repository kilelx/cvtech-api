using FluentValidation;

namespace CVTech.Modules.GestionIdentite.Application.Features.ConnecterUtilisateur;

public class ConnecterUtilisateurQueryValidator : AbstractValidator<ConnecterUtilisateurQuery>
{
    public ConnecterUtilisateurQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.MotDePasse).NotEmpty();
    }
}

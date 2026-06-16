using FluentValidation;

namespace CVTech.Modules.GestionIdentite.Application.Features.InscrireEntreprise;

public class InscrireEntrepriseCommandValidator : AbstractValidator<InscrireEntrepriseCommand>
{
    public InscrireEntrepriseCommandValidator()
    {
        RuleFor(x => x.RaisonSociale).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Siret).NotEmpty().Length(14);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.MotDePasse).NotEmpty().MinimumLength(8);
    }
}

using FluentValidation;

namespace CVTech.Modules.GestionIdentite.Application.Features.InscrireCandidat;

public class InscrireCandidatCommandValidator : AbstractValidator<InscrireCandidatCommand>
{
    public InscrireCandidatCommandValidator()
    {
        RuleFor(x => x.Prenom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.MotDePasse).NotEmpty().MinimumLength(8);
    }
}

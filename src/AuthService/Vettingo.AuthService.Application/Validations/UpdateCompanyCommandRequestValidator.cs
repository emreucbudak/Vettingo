using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.UpdateCompany;

public sealed class UpdateCompanyCommandRequestValidator : AbstractValidator<UpdateCompanyCommandRequest>
{
    public UpdateCompanyCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyDescription).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanySector).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyWebsite).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanySize).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyAddress).NotEmpty().MaximumLength(2000);
    }
}


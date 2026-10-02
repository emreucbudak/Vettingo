using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.CreateCompany;

public sealed class CreateCompanyCommandRequestValidator : AbstractValidator<CreateCompanyCommandRequest>
{
    public CreateCompanyCommandRequestValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyDescription).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanySector).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyWebsite).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanySize).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyAddress).NotEmpty().MaximumLength(2000);
    }
}


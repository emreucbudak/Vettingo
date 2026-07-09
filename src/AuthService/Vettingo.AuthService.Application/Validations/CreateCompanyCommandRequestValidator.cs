using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.CreateCompany;

public sealed class CreateCompanyCommandRequestValidator : AbstractValidator<CreateCompanyCommandRequest>
{
    public CreateCompanyCommandRequestValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyDescription).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyPhone).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompanyEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CompanyAddress).NotEmpty().MaximumLength(2000);
    }
}


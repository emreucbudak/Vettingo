using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany;

public sealed class DeleteCompanyCommandRequestValidator : AbstractValidator<DeleteCompanyCommandRequest>
{
    public DeleteCompanyCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}


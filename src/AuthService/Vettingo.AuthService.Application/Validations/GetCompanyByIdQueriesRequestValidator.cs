using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById;

public sealed class GetCompanyByIdQueriesRequestValidator : AbstractValidator<GetCompanyByIdQueriesRequest>
{
    public GetCompanyByIdQueriesRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}


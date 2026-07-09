using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetAll;

public sealed class GetAllCompaniesQueryRequestValidator : AbstractValidator<GetAllCompaniesQueryRequest>
{
    public GetAllCompaniesQueryRequestValidator()
    {
        // This query has no input fields to validate.
    }
}


using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetByCompany;

namespace Vettingo.JobService.Application.Validations;

public sealed class GetCompanyJobPostingsQueryRequestValidator : AbstractValidator<GetCompanyJobPostingsQueryRequest>
{
    public GetCompanyJobPostingsQueryRequestValidator()
    {
        RuleFor(request => request.CompanyId).NotEmpty();
        RuleFor(request => request.Limit).InclusiveBetween(1, 100)
            .When(request => request.Limit.HasValue);
    }
}

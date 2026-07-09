using FluentValidation;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetAll;

public sealed class GetAllJobPostingsQueryRequestValidator : AbstractValidator<GetAllJobPostingsQueryRequest>
{
    public GetAllJobPostingsQueryRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().When(x => x.CompanyId.HasValue);
    }
}


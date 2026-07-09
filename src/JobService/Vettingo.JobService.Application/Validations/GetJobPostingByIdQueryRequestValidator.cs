using FluentValidation;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById;

public sealed class GetJobPostingByIdQueryRequestValidator : AbstractValidator<GetJobPostingByIdQueryRequest>
{
    public GetJobPostingByIdQueryRequestValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
    }
}


using FluentValidation;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance;

public sealed class GetJobPostingPerformanceQueryRequestValidator : AbstractValidator<GetJobPostingPerformanceQueryRequest>
{
    public GetJobPostingPerformanceQueryRequestValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
    }
}


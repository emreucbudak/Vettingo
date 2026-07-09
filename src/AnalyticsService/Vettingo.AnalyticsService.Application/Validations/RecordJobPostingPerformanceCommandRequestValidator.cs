using FluentValidation;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordJobPostingPerformance;

public sealed class RecordJobPostingPerformanceCommandRequestValidator : AbstractValidator<RecordJobPostingPerformanceCommandRequest>
{
    public RecordJobPostingPerformanceCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.JobPostingId).NotEmpty();
        RuleFor(x => x.ViewCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ApplicationCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RecommendationCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.HiredRecommendationCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CvViewCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MatchCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TopTenPercentMatchCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AverageCompatibilityRate).InclusiveBetween(0, 100);
    }
}


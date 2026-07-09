using FluentValidation;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateCvAnalysis;

public sealed class RecordCandidateCvAnalysisCommandRequestValidator : AbstractValidator<RecordCandidateCvAnalysisCommandRequest>
{
    public RecordCandidateCvAnalysisCommandRequestValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty();
        RuleFor(x => x.PeriodStart).NotEmpty();
        RuleFor(x => x.PeriodEnd).NotEmpty();
        RuleFor(x => x.AiMatchedJobCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TopTenPercentJobCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.HrViewCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MatchCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AverageMatchRate).InclusiveBetween(0, 100);
    }
}


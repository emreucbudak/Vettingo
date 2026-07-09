using FluentValidation;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateRecommendation;

public sealed class RecordCandidateRecommendationCommandRequestValidator : AbstractValidator<RecordCandidateRecommendationCommandRequest>
{
    public RecordCandidateRecommendationCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.JobPostingId).NotEmpty();
        RuleFor(x => x.CandidateId).NotEmpty();
        RuleFor(x => x.CandidateName).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CompatibilityRate).InclusiveBetween(0, 100);
        RuleFor(x => x.RecommendedAt).NotEmpty();
        RuleFor(x => x.HiredAt).NotEmpty();
    }
}


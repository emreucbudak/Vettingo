using FluentValidation;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCandidateCvAnalysis;

public sealed class GetCandidateCvAnalysisQueryRequestValidator : AbstractValidator<GetCandidateCvAnalysisQueryRequest>
{
    public GetCandidateCvAnalysisQueryRequestValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty();
    }
}


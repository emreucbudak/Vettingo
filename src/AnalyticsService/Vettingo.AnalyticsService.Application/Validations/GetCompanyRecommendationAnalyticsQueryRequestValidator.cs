using FluentValidation;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyRecommendationAnalytics;

public sealed class GetCompanyRecommendationAnalyticsQueryRequestValidator : AbstractValidator<GetCompanyRecommendationAnalyticsQueryRequest>
{
    public GetCompanyRecommendationAnalyticsQueryRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}


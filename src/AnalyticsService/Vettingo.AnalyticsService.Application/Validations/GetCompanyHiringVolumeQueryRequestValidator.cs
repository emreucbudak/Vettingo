using FluentValidation;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyHiringVolume;

namespace Vettingo.AnalyticsService.Application.Validations;

public sealed class GetCompanyHiringVolumeQueryRequestValidator : AbstractValidator<GetCompanyHiringVolumeQueryRequest>
{
    public GetCompanyHiringVolumeQueryRequestValidator()
    {
        RuleFor(request => request.CompanyId).NotEmpty();
    }
}

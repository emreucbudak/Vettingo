using FluentValidation;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyCandidateSource;

namespace Vettingo.AnalyticsService.Application.Validations;

public sealed class GetCompanyCandidateSourceQueryRequestValidator : AbstractValidator<GetCompanyCandidateSourceQueryRequest>
{
    public GetCompanyCandidateSourceQueryRequestValidator()
    {
        RuleFor(request => request.CompanyId).NotEmpty();
    }
}

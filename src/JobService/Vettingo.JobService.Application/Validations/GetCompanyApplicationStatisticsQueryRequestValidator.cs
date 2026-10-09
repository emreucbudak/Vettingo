using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCompanyStatistics;

namespace Vettingo.JobService.Application.Validations;

public sealed class GetCompanyApplicationStatisticsQueryRequestValidator
    : AbstractValidator<GetCompanyApplicationStatisticsQueryRequest>
{
    public GetCompanyApplicationStatisticsQueryRequestValidator()
    {
        RuleFor(request => request.CompanyId).NotEmpty();
    }
}

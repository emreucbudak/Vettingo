using FluentValidation;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetCompanyStatistics;

namespace Vettingo.InterviewService.Application.Validations;

public sealed class GetCompanyInterviewStatisticsQueryRequestValidator
    : AbstractValidator<GetCompanyInterviewStatisticsQueryRequest>
{
    public GetCompanyInterviewStatisticsQueryRequestValidator()
    {
        RuleFor(request => request.CompanyId).NotEmpty();
    }
}

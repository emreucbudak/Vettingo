using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCandidateStatistics;

namespace Vettingo.JobService.Application.Validations;

public sealed class GetCandidateApplicationStatisticsQueryRequestValidator : AbstractValidator<GetCandidateApplicationStatisticsQueryRequest>
{
    public GetCandidateApplicationStatisticsQueryRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
    }
}

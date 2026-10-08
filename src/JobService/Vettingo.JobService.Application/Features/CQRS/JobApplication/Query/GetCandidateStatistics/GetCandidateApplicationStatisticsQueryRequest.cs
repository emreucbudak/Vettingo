using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCandidateStatistics;

public record GetCandidateApplicationStatisticsQueryRequest : IRequest<GetCandidateApplicationStatisticsQueryResponse>
{
    public Guid UserId { get; init; }
}

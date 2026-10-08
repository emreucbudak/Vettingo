using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCandidateStatistics;

public class GetCandidateApplicationStatisticsQueryHandler(
    IJobApplicationRepository repository,
    ILogger<GetCandidateApplicationStatisticsQueryHandler> logger)
    : IRequestHandler<GetCandidateApplicationStatisticsQueryRequest, GetCandidateApplicationStatisticsQueryResponse>
{
    public async Task<GetCandidateApplicationStatisticsQueryResponse> Handle(
        GetCandidateApplicationStatisticsQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetCandidateApplicationStatisticsQueryHandler));
        var statistics = await repository.GetCandidateDashboardStatisticsAsync(request.UserId, cancellationToken);
        return new(statistics.TotalApplications, statistics.InProgress, statistics.Offers, statistics.Rejected);
    }
}

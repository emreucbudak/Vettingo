using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetStatistics;

public sealed class GetJobPostingStatisticsQueryHandler(IJobPostingRepository repository)
    : IRequestHandler<GetJobPostingStatisticsQueryRequest, JobPostingStatistics>
{
    public Task<JobPostingStatistics> Handle(GetJobPostingStatisticsQueryRequest request, CancellationToken cancellationToken)
        => repository.GetStatisticsAsync(request.CompanyId, cancellationToken);
}

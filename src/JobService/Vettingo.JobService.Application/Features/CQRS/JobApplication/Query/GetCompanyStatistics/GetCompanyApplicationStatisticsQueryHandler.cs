using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCompanyStatistics;

public sealed class GetCompanyApplicationStatisticsQueryHandler(IJobApplicationRepository repository)
    : IRequestHandler<GetCompanyApplicationStatisticsQueryRequest, GetCompanyApplicationStatisticsQueryResponse>
{
    public async Task<GetCompanyApplicationStatisticsQueryResponse> Handle(
        GetCompanyApplicationStatisticsQueryRequest request, CancellationToken cancellationToken)
    {
        var statistics = await repository.GetCompanyStatisticsAsync(request.CompanyId, cancellationToken);
        return new(statistics.TotalApplications, statistics.UnderReview, statistics.Interviews,
            statistics.Offers, statistics.Rejected);
    }
}

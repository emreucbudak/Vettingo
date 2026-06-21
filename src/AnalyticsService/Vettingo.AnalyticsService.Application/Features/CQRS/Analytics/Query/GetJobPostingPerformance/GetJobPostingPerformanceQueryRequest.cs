using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance
{
    public class GetJobPostingPerformanceQueryRequest : IRequest<GetJobPostingPerformanceQueryResponse>
    {
        public Guid JobPostingId { get; init; }
    }
}

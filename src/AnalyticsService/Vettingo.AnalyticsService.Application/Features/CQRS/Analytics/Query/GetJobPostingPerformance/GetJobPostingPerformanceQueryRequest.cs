using FlashMediator;

using Vettingo.AnalyticsService.Application.Interfaces;
namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance
{
    public class GetJobPostingPerformanceQueryRequest : IRequest<GetJobPostingPerformanceQueryResponse>, ICacheableQuery
    {
        public Guid JobPostingId { get; init; }

        public GetJobPostingPerformanceQueryRequest(Guid jobPostingId)
        {
            JobPostingId = jobPostingId;
            CacheKey = JobPostingId.ToString();
            ExpirationTime = TimeSpan.FromMinutes(10);
        }

        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}

using FlashMediator;
using Vettingo.AnalyticsService.Application.Interfaces;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCandidateCvAnalysis
{
    public class GetCandidateCvAnalysisQueryRequest : IRequest<IEnumerable<GetCandidateCvAnalysisQueryResponse>> , ICacheableQuery
    {
        public GetCandidateCvAnalysisQueryRequest(Guid candidateId, bool latestOnly)
        {
            CandidateId = candidateId;
            LatestOnly = latestOnly;
            CacheKey = CandidateId.ToString();
            ExpirationTime = TimeSpan.FromMinutes(10);
        }

        public Guid CandidateId { get; init; }
        public bool LatestOnly { get; init; }
        public string CacheKey { get; set; } 
        public TimeSpan ExpirationTime {  get; set; }
    }
}

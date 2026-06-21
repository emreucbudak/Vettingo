using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCandidateCvAnalysis
{
    public class GetCandidateCvAnalysisQueryRequest : IRequest<IEnumerable<GetCandidateCvAnalysisQueryResponse>>
    {
        public Guid CandidateId { get; init; }
        public bool LatestOnly { get; init; }
    }
}

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyCandidateSource;

public sealed class GetCompanyCandidateSourceQueryResponse
{
    public int Scout { get; init; }
    public int Application { get; init; }
    public int CandidateCount { get; init; }
}

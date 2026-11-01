namespace Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Query.GetCompanyCandidateSource;

public sealed record GetCompanyCandidateSourceQueryResponse
{
    public int Scout { get; init; }
    public int Application { get; init; }
    public int CandidateCount { get; init; }
}

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyHiringVolume;

public sealed class GetCompanyHiringVolumeQueryResponse
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int HireCount { get; init; }
}

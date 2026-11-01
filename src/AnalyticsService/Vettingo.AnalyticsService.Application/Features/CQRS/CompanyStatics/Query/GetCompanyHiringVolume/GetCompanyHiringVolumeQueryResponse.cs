namespace Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Query.GetCompanyHiringVolume;

public sealed record GetCompanyHiringVolumeQueryResponse
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int HireCount { get; init; }
}

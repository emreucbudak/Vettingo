using FlashMediator;
using Vettingo.JobService.Application.Interfaces;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.Search;

public sealed record SearchJobPostingsQueryRequest
    : IRequest<IReadOnlyList<SearchJobPostingsQueryResponse>>, ICacheableQuery
{
    private string? _cacheKey;

    public string? Title { get; init; }
    public string? Location { get; init; }
    public EmploymentType? EmploymentType { get; init; }
    public WorkingModel? WorkingModel { get; init; }
    public ExperienceLevel? ExperienceLevel { get; init; }
    public decimal? MinSalary { get; init; }
    public decimal? MaxSalary { get; init; }

    public string CacheKey
    {
        get => _cacheKey ?? BuildCacheKey();
        set => _cacheKey = value;
    }

    public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromMinutes(5);

    private string BuildCacheKey()
    {
        return string.Join(
            ':',
            "SearchJobPostings",
            Normalize(Title),
            Normalize(Location),
            EmploymentType?.ToString() ?? "all",
            WorkingModel?.ToString() ?? "all",
            ExperienceLevel?.ToString() ?? "all",
            MinSalary?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "all",
            MaxSalary?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "all");
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "all"
            : value.Trim().ToLowerInvariant();
    }
}

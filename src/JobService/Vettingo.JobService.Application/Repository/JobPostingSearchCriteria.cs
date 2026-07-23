using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Repository;

public sealed record JobPostingSearchCriteria
{
    public string? Title { get; init; }
    public string? Location { get; init; }
    public EmploymentType? EmploymentType { get; init; }
    public WorkingModel? WorkingModel { get; init; }
    public ExperienceLevel? ExperienceLevel { get; init; }
    public decimal? MinSalary { get; init; }
    public decimal? MaxSalary { get; init; }
}

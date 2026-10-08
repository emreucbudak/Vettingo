using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Repository;

public sealed record JobPostingSearchCriteria
{
    public string? Title { get; init; }
    public int? CityId { get; init; }
    public EmploymentType? EmploymentType { get; init; }
    public WorkingModel? WorkingModel { get; init; }
    public ExperienceLevel? ExperienceLevel { get; init; }
    public int? MinSalary { get; init; }
    public int? MaxSalary { get; init; }
}

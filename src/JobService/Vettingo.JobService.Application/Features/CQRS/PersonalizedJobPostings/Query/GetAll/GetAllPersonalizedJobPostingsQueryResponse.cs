using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

public sealed record GetAllPersonalizedJobPostingsQueryResponse
{
    public Guid JobPostingId { get; init; }
    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string CityName { get; init; } = string.Empty;
    public EmploymentType EmploymentType { get; init; }
    public WorkingModel WorkingModel { get; init; }
    public int MinimumExperienceYear { get; init; }
    public int MaximumExperienceYear { get; init; }
    public int MinSalary { get; init; }
    public int MaxSalary { get; init; }
    public Dictionary<string, int> RequirementSkills { get; init; } = new();
}

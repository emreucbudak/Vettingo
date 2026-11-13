using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.DTOs;

public sealed class JobPostingPublishedMessage
{
    public Guid JobPostingId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int CityId { get; init; }
    public EmploymentType EmploymentType { get; init; }
    public WorkingModel WorkingModel { get; init; }
    public int MinimumExperienceYear { get; init; }
    public int MaximumExperienceYear { get; init; }
    public int MinSalary { get; init; }
    public int MaxSalary { get; init; }
    public IReadOnlyCollection<StepMessageDto> CandidateSkills { get; init; } = [];
}

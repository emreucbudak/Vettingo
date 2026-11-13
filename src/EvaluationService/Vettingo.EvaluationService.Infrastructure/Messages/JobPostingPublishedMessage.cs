namespace Vettingo.EvaluationService.Infrastructure.Messages;

public sealed class JobPostingPublishedMessage
{
    public Guid JobPostingId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int CityId { get; init; }
    public int EmploymentType { get; init; }
    public int WorkingModel { get; init; }
    public int MinimumExperienceYear { get; init; }
    public int MaximumExperienceYear { get; init; }
    public int MinSalary { get; init; }
    public int MaxSalary { get; init; }
    public IReadOnlyCollection<StepMessageDto> CandidateSkills { get; init; } = [];
}

public sealed record StepMessageDto(string SkillName, int ExperienceYear);

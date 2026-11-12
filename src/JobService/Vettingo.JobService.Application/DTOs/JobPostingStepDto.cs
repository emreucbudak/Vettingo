using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.DTOs;

public record JobPostingStepDto
{
    public int Order { get; init; }
    public StepType StepType { get; init; }
    public Guid? ExamId { get; init; }
    public DateOnly StartedDate { get; init; }
    public DateOnly EndDate { get; init; }
    public TimeOnly? StartingLimitTime { get; init; }
    public TimeOnly? StoppedLimitTime { get; init; }
    public int? InterviewIntervalMinutes { get; init; }
    public string? InterviewChapterName { get; init; }
    public string? MeetingLink { get; init; }
}

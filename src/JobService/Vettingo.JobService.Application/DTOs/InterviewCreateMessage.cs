namespace Vettingo.JobService.Application.DTOs;

public sealed class InterviewCreateMessage
{
    public required Guid JobId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public TimeOnly? StartingLimitTime { get; init; }
    public TimeOnly? StoppedLimitTime { get; init; }
    public string? InterviewChapterName { get; init; }
    public string? MeetingLink { get; init; }
    public required List<Guid> CandidateIds { get; init; }
}

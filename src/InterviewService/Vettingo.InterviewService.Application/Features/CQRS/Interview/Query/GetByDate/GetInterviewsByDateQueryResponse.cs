namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;

public sealed record GetInterviewsByDateQueryResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateOnly InterviewDate { get; init; }
    public TimeOnly StartedTime { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public string Chapter { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string WhereIsMeeting { get; init; } = string.Empty;
    public string? MeetingLink { get; init; }
}

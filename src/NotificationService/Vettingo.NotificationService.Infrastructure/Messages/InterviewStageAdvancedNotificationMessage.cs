namespace Vettingo.NotificationService.Infrastructure.Messages;

public sealed class InterviewStageAdvancedNotificationMessage
{
    public required Guid Id { get; init; }
    public required Guid CandidateId { get; init; }
    public required string Email { get; init; }
    public required string CompanyName { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public string? InterviewChapterName { get; init; }
}

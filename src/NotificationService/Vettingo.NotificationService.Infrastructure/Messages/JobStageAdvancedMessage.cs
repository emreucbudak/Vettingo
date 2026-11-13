namespace Vettingo.NotificationService.Infrastructure.Messages;

public sealed class JobStageAdvancedMessage
{
    public required Guid CandidateId { get; init; }
    public required string Email { get; init; }
    public required string CompanyName { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required string StageType { get; init; }
}

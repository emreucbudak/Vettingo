namespace Vettingo.AuthService.Infrastructure.Messages;

public sealed class ExamStageAdvancedRequestedMessage
{
    public const string TopicName = "vettingo.auth.exam-stage-advanced.requested.v1";

    public required IReadOnlyDictionary<Guid, Guid> NotificationIdsByCandidateId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
}

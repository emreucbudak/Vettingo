namespace Vettingo.AuthService.Infrastructure.Messages;

public sealed class InterviewStageExtendedRequestedMessage
{
    public const string TopicName = "vettingo.auth.interview-stage-extended.notification.requested.v1";

    public required Guid CandidateId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required int AdditionalDays { get; init; }
    public required DateOnly EndDate { get; init; }
}

namespace Vettingo.AuthService.Infrastructure.Messages;

public sealed class JobStageAdvancedRequestedMessage
{
    public const string TopicName = "vettingo.auth.job-stage-advanced.requested.v1";

    public required IReadOnlyList<Guid> CandidateIds { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required string StageType { get; init; }
}

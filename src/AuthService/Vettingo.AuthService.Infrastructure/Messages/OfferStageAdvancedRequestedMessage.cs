namespace Vettingo.AuthService.Infrastructure.Messages;

public sealed class OfferStageAdvancedRequestedMessage
{
    public const string TopicName = "vettingo.auth.offer-stage-advanced.requested.v1";

    public required IReadOnlyDictionary<Guid, Guid> NotificationIdsByCandidateId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
}

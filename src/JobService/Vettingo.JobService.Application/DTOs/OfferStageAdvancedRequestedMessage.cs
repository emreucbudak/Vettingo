namespace Vettingo.JobService.Application.DTOs;

public sealed class OfferStageAdvancedRequestedMessage
{
    public required IReadOnlyDictionary<Guid, Guid> NotificationIdsByCandidateId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
}

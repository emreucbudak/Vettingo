using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;

public sealed record SendJobStageAdvancedNotificationQueryRequest : IRequest
{
    public required IReadOnlyList<Guid> CandidateIds { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required string StageType { get; init; }
}

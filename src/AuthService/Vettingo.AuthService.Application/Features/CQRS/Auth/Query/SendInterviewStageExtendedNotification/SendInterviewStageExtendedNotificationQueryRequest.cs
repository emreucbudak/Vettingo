using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendInterviewStageExtendedNotification;

public sealed record SendInterviewStageExtendedNotificationQueryRequest : IRequest
{
    public required Guid CandidateId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required int AdditionalDays { get; init; }
    public required DateOnly EndDate { get; init; }
}

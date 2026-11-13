namespace Vettingo.NotificationService.Infrastructure.Messages;

public sealed class OfferStageAdvancedNotificationMessage
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Email { get; init; }
    public required string CompanyName { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
}

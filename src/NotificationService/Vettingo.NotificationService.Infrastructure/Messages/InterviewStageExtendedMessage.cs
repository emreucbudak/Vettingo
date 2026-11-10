namespace Vettingo.NotificationService.Infrastructure.Messages;

public sealed class InterviewStageExtendedMessage
{
    public required string Email { get; init; }
    public required string CompanyName { get; init; }
    public required string Title { get; init; }
    public required int AdditionalDays { get; init; }
    public required DateOnly EndDate { get; init; }
}

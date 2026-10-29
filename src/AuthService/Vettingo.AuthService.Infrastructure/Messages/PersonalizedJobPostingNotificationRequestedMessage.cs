namespace Vettingo.AuthService.Infrastructure.Messages;

public sealed record PersonalizedJobPostingNotificationRequestedMessage
{
    public const string TopicName = "vettingo.auth.personalized-job-posting.notification.requested.v1";

    public Guid Id { get; init; }
    public required Guid UserId { get; init; }
}

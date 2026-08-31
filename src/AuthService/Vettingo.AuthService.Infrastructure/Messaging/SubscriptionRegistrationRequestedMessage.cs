namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed record SubscriptionRegistrationRequestedMessage
{
    public const string TopicName = "vettingo.auth.registration.requested.v1";

    public required string AccountType { get; init; }
    public required Guid RegistrationToken { get; init; }
    public required Guid SubscriberId { get; init; }
}

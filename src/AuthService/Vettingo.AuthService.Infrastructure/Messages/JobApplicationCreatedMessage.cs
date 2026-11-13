namespace Vettingo.AuthService.Infrastructure.Messages;

public sealed class JobApplicationCreatedMessage
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
}

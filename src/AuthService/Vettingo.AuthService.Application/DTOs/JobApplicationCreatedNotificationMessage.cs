namespace Vettingo.AuthService.Application.DTOs;

public sealed class JobApplicationCreatedNotificationMessage
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}

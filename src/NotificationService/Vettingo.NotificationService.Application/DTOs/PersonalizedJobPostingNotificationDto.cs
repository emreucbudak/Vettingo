namespace Vettingo.NotificationService.Application.DTOs;

public sealed record PersonalizedJobPostingNotificationDto
{
    public Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}

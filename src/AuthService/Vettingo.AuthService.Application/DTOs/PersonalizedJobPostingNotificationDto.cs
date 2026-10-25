namespace Vettingo.AuthService.Application.DTOs;

public sealed record PersonalizedJobPostingNotificationDto
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}

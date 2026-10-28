namespace Vettingo.JobService.Application.DTOs;

public sealed record PersonalizedJobPostingNotificationDto
{
    public Guid Id { get; init; }
    public required Guid UserId { get; init; }
}

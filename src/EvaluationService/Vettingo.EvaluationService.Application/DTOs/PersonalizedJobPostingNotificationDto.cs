namespace Vettingo.EvaluationService.Application.DTOs;

public sealed record PersonalizedJobPostingNotificationDto
{
    public required Guid UserId { get; init; }
}

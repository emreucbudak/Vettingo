namespace Vettingo.JobService.Application.DTOs;

public sealed record PersonalizedJobPostingNotificationMessage
{
    public Guid Id { get; init; }
    public  Guid UserId { get; init; }
}

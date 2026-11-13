namespace Vettingo.JobService.Application.DTOs;

public sealed class JobApplicationCreatedMessage
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
}

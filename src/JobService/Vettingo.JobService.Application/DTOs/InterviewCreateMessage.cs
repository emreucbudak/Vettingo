namespace Vettingo.JobService.Application.DTOs;

public sealed class InterviewCreateMessage
{
    public required Guid JobId { get; init; }
    public required DateTime StartedDate { get; init; }
    public required DateTime EndDate { get; init; }
}

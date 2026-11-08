namespace Vettingo.JobService.Infrastructure.Messages;

public sealed class InterviewResultMessage
{
    public required Guid UserId { get; init; }
    public required Guid JobId { get; init; }
}

namespace Vettingo.InterviewService.Application.DTOs;

public sealed class InterviewResultMessage
{
    public required Guid UserId { get; init; }
    public required Guid JobId { get; init; }
}

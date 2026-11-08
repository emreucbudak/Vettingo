namespace Vettingo.ExamService.Application.DTOs;

public sealed class ExamResultMessage
{
    public required Guid UserId { get; init; }
    public required Guid JobId { get; init; }
}

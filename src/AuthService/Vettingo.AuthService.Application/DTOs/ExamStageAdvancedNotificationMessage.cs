namespace Vettingo.AuthService.Application.DTOs;

public sealed class ExamStageAdvancedNotificationMessage
{
    public required Guid Id { get; init; }
    public required Guid CandidateId { get; init; }
    public required string Email { get; init; }
    public required string CompanyName { get; init; }
    public required string Title { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
}

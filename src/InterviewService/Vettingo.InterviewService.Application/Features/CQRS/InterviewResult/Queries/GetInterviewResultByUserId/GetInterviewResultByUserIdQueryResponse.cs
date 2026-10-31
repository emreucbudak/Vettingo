namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetInterviewResultByUserId;

public record GetInterviewResultByUserIdQueryResponse
{
    public string Chapter { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateOnly InterviewDate { get; init; }
    public int InterviewScore { get; init; }
    public bool Passed { get; init; }
}

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetAllInterviewResultByInterviewId;

public record GetAllInterviewResultByInterviewIdQueryResponse
{
    public Guid UserId { get; init; }
    public int InterviewScore { get; init; }
    public bool Passed { get; init; }
}

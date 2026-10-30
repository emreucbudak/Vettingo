namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetInterviewResultByUserId;

public record GetInterviewResultByUserIdQueryResponse
{
    public Guid InterviewId { get; init; }
    public int InterviewScore { get; init; }
    public bool Passed { get; init; }
}

using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetAllInterviewResultByInterviewId;

public record GetAllInterviewResultByInterviewIdQueryRequest : IRequest<IEnumerable<GetAllInterviewResultByInterviewIdQueryResponse>>
{
    public Guid InterviewId { get; init; }
}

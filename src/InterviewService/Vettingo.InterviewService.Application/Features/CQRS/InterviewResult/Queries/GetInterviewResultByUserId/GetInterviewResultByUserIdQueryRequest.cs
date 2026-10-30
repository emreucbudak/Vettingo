using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetInterviewResultByUserId;

public record GetInterviewResultByUserIdQueryRequest : IRequest<IEnumerable<GetInterviewResultByUserIdQueryResponse>>
{
    public Guid UserId { get; init; }
}

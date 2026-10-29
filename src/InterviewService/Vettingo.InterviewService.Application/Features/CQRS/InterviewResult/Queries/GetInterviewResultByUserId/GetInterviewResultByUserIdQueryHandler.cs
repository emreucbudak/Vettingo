using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetInterviewResultByUserId;

public class GetInterviewResultByUserIdQueryHandler
    : IRequestHandler<GetInterviewResultByUserIdQueryRequest, GetInterviewResultByUserIdQueryResponse>
{
    public Task<GetInterviewResultByUserIdQueryResponse> Handle(
        GetInterviewResultByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

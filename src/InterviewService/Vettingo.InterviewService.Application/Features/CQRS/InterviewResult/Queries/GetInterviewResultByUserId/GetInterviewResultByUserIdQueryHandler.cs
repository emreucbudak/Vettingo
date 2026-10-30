using FlashMediator;
using Vettingo.InterviewService.Application.Repository;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetInterviewResultByUserId;

public class GetInterviewResultByUserIdQueryHandler(IInterviewResultRepository repository)
    : IRequestHandler<GetInterviewResultByUserIdQueryRequest, IEnumerable<GetInterviewResultByUserIdQueryResponse>>
{
    public async Task<IEnumerable<GetInterviewResultByUserIdQueryResponse>> Handle(
        GetInterviewResultByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        var results = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        return results.Select(result => new GetInterviewResultByUserIdQueryResponse
        {
            InterviewId = result.InterviewId,
            InterviewScore = result.InterviewScore,
            Passed = result.Passed
        }).ToList();
    }
}

using FlashMediator;
using Vettingo.InterviewService.Application.Repository;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetAllInterviewResultByInterviewId;

public class GetAllInterviewResultByInterviewIdQueryHandler(IInterviewResultRepository repository)
    : IRequestHandler<GetAllInterviewResultByInterviewIdQueryRequest, IEnumerable<GetAllInterviewResultByInterviewIdQueryResponse>>
{
    public async Task<IEnumerable<GetAllInterviewResultByInterviewIdQueryResponse>> Handle(
        GetAllInterviewResultByInterviewIdQueryRequest request, CancellationToken cancellationToken)
    {
        var results = await repository.GetAllByInterviewIdAsync(request.InterviewId, cancellationToken);

        return results.Select(result => new GetAllInterviewResultByInterviewIdQueryResponse
        {
            UserId = result.UserId,
            InterviewScore = result.InterviewScore,
            Passed = result.Passed
        }).ToList();
    }
}

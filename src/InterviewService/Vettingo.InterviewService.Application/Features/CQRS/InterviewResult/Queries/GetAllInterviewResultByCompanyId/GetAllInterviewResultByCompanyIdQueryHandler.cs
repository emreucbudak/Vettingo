using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetAllInterviewResultByCompanyId;

public class GetAllInterviewResultByCompanyIdQueryHandler
    : IRequestHandler<GetAllInterviewResultByCompanyIdQueryRequest, IEnumerable<GetAllInterviewResultByCompanyIdQueryResponse>>
{
    public Task<IEnumerable<GetAllInterviewResultByCompanyIdQueryResponse>> Handle(
        GetAllInterviewResultByCompanyIdQueryRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

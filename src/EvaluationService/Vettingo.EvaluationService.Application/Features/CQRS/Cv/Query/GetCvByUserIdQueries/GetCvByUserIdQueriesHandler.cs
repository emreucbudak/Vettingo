using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.GetCvByUserIdQueries;

public sealed class GetCvByUserIdQueriesHandler
    : IRequestHandler<GetCvByUserIdQueriesRequest, GetCvByUserIdQueriesResponse>
{
    public Task<GetCvByUserIdQueriesResponse> Handle(
        GetCvByUserIdQueriesRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

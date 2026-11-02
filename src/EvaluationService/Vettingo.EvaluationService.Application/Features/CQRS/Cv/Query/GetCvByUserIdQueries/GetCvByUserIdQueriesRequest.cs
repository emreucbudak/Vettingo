using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.GetCvByUserIdQueries;

public sealed class GetCvByUserIdQueriesRequest : IRequest<GetCvByUserIdQueriesResponse>
{
    public Guid UserId { get; init; }
}

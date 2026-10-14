using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Scout.Query.GetScoutByFilter;

public sealed class GetScoutByFilterQueryHandler
    : IRequestHandler<GetScoutByFilterQueryRequest, GetScoutByFilterQueryResponse>
{
    public Task<GetScoutByFilterQueryResponse> Handle(
        GetScoutByFilterQueryRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

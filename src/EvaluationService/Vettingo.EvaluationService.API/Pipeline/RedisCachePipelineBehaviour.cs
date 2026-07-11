using FlashMediator;
using Vettingo.EvaluationService.Application.Interfaces;

namespace Vettingo.EvaluationService.API.Pipeline;

public sealed class RedisCachePipelineBehaviour<TRequest, TResponse>(ICacheService cache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery query)
        {
            return await next();
        }

        TResponse? cachedResponse = await cache.Get<TResponse>(query.CacheKey);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        TResponse response = await next();
        await cache.Set(query.CacheKey, response, query.ExpirationTime);
        return response;
    }
}

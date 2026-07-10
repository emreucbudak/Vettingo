using FlashMediator;
using Vettingo.AnalyticsService.Application.Interfaces;

namespace Vettingo.AnalyticsService.API.Pipeline
{
    public class RedisCachePipelineBehaviour<TRequest, TResponse>(ICacheService cache) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(request is not ICacheableQuery query)
            {
                return await next();
            }
            var data = await cache.Get<TResponse>(query.CacheKey);
            if (data is null)
            {
                var response = await next();
                await cache.Set(query.CacheKey, response, query.ExpirationTime);
                return response;
            }
            else
            {
                return data;
            }

        }
    }
}

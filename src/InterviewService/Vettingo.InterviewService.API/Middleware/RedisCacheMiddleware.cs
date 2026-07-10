using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FlashMediator;
using Microsoft.AspNetCore.Mvc.Controllers;
using Vettingo.InterviewService.Application.Interfaces;

namespace Vettingo.InterviewService.API.Middleware
{

    public class RedisCacheMiddleware<TRequest, TResponse> :IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICacheService _cacheService;
        public RedisCacheMiddleware(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICacheableQuery query)
            {
                return await next();
            }
            var data = await _cacheService.Get<TResponse>(query.CacheKey);
            if (data is null)
            {
                var response = await next();
                await _cacheService.Set(query.CacheKey, response, query.ExpirationTime);
                return response;
            }
            return data;
        }
    }
}

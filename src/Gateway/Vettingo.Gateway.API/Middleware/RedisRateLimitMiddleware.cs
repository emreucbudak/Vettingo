using Vettingo.Gateway.API.Interface;

namespace Vettingo.Gateway.API.Middleware
{
    public  class RedisRateLimitMiddleware(IRedisRateLimiter redisRateLimiter) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var email = context.User?.FindFirst(c => c.Type == "email")?.Value;
            if(email is null)
            {
                throw new Exception("İstediğiniz işlem için giriş yapmalısınız.");
            }
            bool check = await redisRateLimiter.CheckRateLimit(email);
            if (!check)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Çok fazla istek gönderdiniz. Lütfen daha sonra tekrar deneyin.");
            }
            await next(context);

        }
    }
}

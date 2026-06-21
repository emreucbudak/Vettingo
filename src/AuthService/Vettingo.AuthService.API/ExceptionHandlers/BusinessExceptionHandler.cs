using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;

namespace Vettingo.AuthService.API.ExceptionHandlers
{
    public class BusinessExceptionHandler(ILogger<BusinessExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not BusinessException typedException)
            {
                return false;
            }

            return await ExceptionResponseWriter.WriteAsync(httpContext, logger, typedException, StatusCodes.Status422UnprocessableEntity, typedException.Message, cancellationToken);
        }
    }
}

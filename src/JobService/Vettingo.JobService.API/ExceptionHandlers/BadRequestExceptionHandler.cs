using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Exceptions;

namespace Vettingo.JobService.API.ExceptionHandlers
{
    public class BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not BadRequestException typedException)
            {
                return false;
            }

            return await ExceptionResponseWriter.WriteAsync(httpContext, logger, typedException, StatusCodes.Status400BadRequest, typedException.Message, cancellationToken);
        }
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Bases;

namespace Vettingo.JobService.API.ExceptionHandlers
{
    public class BaseExceptionHandler(ILogger<BaseExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not BaseException baseException)
            {
                return false;
            }

            return await ExceptionResponseWriter.WriteAsync(httpContext, logger, baseException, StatusCodes.Status400BadRequest, baseException.Message, cancellationToken);
        }
    }
}

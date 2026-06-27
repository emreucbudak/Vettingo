using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Bases;

namespace Vettingo.InterviewService.API.ExceptionHandlers
{
    public class BaseExceptionHandler(ILogger<BaseExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not BaseException typedException)
            {
                return false;
            }

            return await ExceptionResponseWriter.WriteAsync(httpContext, logger, typedException, StatusCodes.Status400BadRequest, typedException.Message, cancellationToken);
        }
    }
}

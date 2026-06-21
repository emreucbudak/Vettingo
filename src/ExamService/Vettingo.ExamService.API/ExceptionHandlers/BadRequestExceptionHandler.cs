using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Exceptions;

namespace Vettingo.ExamService.API.ExceptionHandlers
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

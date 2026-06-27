using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Vettingo.InterviewService.API.ExceptionHandlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            return await ExceptionResponseWriter.WriteAsync(httpContext, logger, exception, StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu", cancellationToken);
        }
    }
}

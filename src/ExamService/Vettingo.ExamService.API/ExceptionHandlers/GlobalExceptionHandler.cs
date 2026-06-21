using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Vettingo.ExamService.API.ExceptionHandlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            logger.LogError(exception, "{Path} isteğinde beklenmeyen bir hata oluştu", httpContext.Request.Path.Value);

            var response = new Dictionary<string, object?>
            {
                ["statusCode"] = StatusCodes.Status500InternalServerError,
                ["message"] = "Beklenmeyen bir hata oluştu.",
                ["path"] = httpContext.Request.Path.Value,
                ["traceId"] = httpContext.TraceIdentifier
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}


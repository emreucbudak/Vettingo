using Microsoft.AspNetCore.Diagnostics;

namespace Vettingo.EvaluationService.API.ExceptionHandlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        return ExceptionResponseWriter.WriteAsync(
            context, logger, exception, StatusCodes.Status500InternalServerError,
            "Beklenmeyen bir hata oluştu.", cancellationToken);
    }
}

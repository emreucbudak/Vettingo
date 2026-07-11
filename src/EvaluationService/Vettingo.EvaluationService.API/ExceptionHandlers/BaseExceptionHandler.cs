using Microsoft.AspNetCore.Diagnostics;
using Vettingo.EvaluationService.Application.Bases;

namespace Vettingo.EvaluationService.API.ExceptionHandlers;

public sealed class BaseExceptionHandler(ILogger<BaseExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BaseException baseException)
        {
            return false;
        }

        return await ExceptionResponseWriter.WriteAsync(
            context, logger, baseException, StatusCodes.Status500InternalServerError,
            baseException.Message, cancellationToken);
    }
}

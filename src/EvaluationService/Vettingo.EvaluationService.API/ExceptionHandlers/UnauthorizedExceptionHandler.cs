using Microsoft.AspNetCore.Diagnostics;
using Vettingo.EvaluationService.Application.Exceptions;

namespace Vettingo.EvaluationService.API.ExceptionHandlers;

public sealed class UnauthorizedExceptionHandler(ILogger<UnauthorizedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedException unauthorizedException)
        {
            return false;
        }

        return await ExceptionResponseWriter.WriteAsync(
            context, logger, unauthorizedException, StatusCodes.Status401Unauthorized,
            unauthorizedException.Message, cancellationToken);
    }
}

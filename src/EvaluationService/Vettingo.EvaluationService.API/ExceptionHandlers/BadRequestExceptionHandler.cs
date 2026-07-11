using Microsoft.AspNetCore.Diagnostics;
using Vettingo.EvaluationService.Application.Exceptions;

namespace Vettingo.EvaluationService.API.ExceptionHandlers;

public sealed class BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadRequestException badRequestException)
        {
            return false;
        }

        return await ExceptionResponseWriter.WriteAsync(
            context, logger, badRequestException, StatusCodes.Status400BadRequest,
            badRequestException.Message, cancellationToken);
    }
}

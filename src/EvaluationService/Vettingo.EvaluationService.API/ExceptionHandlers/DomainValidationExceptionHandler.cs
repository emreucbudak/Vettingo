using Microsoft.AspNetCore.Diagnostics;

namespace Vettingo.EvaluationService.API.ExceptionHandlers;

public sealed class DomainValidationExceptionHandler(ILogger<DomainValidationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ArgumentException argumentException)
        {
            return false;
        }

        return await ExceptionResponseWriter.WriteAsync(
            context, logger, argumentException, StatusCodes.Status400BadRequest,
            argumentException.Message, cancellationToken);
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Vettingo.EvaluationService.Application.Exceptions;

namespace Vettingo.EvaluationService.API.ExceptionHandlers;

public sealed class BusinessExceptionHandler(ILogger<BusinessExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BusinessException businessException)
        {
            return false;
        }

        return await ExceptionResponseWriter.WriteAsync(
            context, logger, businessException, StatusCodes.Status409Conflict,
            businessException.Message, cancellationToken);
    }
}

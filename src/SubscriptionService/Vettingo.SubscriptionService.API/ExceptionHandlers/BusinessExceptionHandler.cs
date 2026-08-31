using Microsoft.AspNetCore.Diagnostics;
using Vettingo.SubscriptionService.Application.Exceptions;

namespace Vettingo.SubscriptionService.API.ExceptionHandlers;

public sealed class BusinessExceptionHandler(
    ILogger<BusinessExceptionHandler> logger)
    : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BusinessException typedException)
        {
            return ValueTask.FromResult(false);
        }

        return ExceptionResponseWriter.WriteAsync(
            context,
            logger,
            typedException,
            StatusCodes.Status422UnprocessableEntity,
            typedException.Message,
            cancellationToken);
    }
}

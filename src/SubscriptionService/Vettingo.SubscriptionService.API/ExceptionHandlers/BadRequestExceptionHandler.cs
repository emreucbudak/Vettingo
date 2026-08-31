using Microsoft.AspNetCore.Diagnostics;
using Vettingo.SubscriptionService.Application.Exceptions;

namespace Vettingo.SubscriptionService.API.ExceptionHandlers;

public sealed class BadRequestExceptionHandler(
    ILogger<BadRequestExceptionHandler> logger)
    : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadRequestException typedException)
        {
            return ValueTask.FromResult(false);
        }

        return ExceptionResponseWriter.WriteAsync(
            context,
            logger,
            typedException,
            StatusCodes.Status400BadRequest,
            typedException.Message,
            cancellationToken);
    }
}

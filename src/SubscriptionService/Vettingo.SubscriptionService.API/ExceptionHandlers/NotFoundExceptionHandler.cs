using Microsoft.AspNetCore.Diagnostics;
using Vettingo.SubscriptionService.Application.Exceptions;

namespace Vettingo.SubscriptionService.API.ExceptionHandlers;

public sealed class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException notFoundException)
        {
            return false;
        }

        return await ExceptionResponseWriter.WriteAsync(
            context,
            logger,
            notFoundException,
            StatusCodes.Status404NotFound,
            notFoundException.Message,
            cancellationToken);
    }
}

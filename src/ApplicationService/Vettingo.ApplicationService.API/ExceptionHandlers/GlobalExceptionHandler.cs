using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Vettingo.ApplicationService.Application.Exceptions;

namespace Vettingo.ApplicationService.API.ExceptionHandlers
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, message) = exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
                BadRequestException or ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu.")
            };

            httpContext.Response.StatusCode = statusCode;
            logger.LogError(exception, "Başvuru servisi isteği {StatusCode} ile sonuçlandı", statusCode);
            await httpContext.Response.WriteAsJsonAsync(new
            {
                statusCode,
                message,
                path = httpContext.Request.Path.Value,
                traceId = httpContext.TraceIdentifier
            }, cancellationToken);
            return true;
        }
    }
}

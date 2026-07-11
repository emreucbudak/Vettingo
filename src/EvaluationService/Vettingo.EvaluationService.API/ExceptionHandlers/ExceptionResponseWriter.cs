namespace Vettingo.EvaluationService.API.ExceptionHandlers;

internal static class ExceptionResponseWriter
{
    internal static async ValueTask<bool> WriteAsync(
        HttpContext httpContext,
        ILogger logger,
        Exception exception,
        int statusCode,
        string message,
        CancellationToken cancellationToken,
        object? errors = null)
    {
        httpContext.Response.StatusCode = statusCode;
        logger.LogWarning(
            exception,
            "{Path} isteği {StatusCode} durum koduyla başarısız oldu",
            httpContext.Request.Path.Value,
            statusCode);

        Dictionary<string, object?> response = new()
        {
            ["statusCode"] = statusCode,
            ["message"] = message,
            ["path"] = httpContext.Request.Path.Value,
            ["traceId"] = httpContext.TraceIdentifier
        };

        if (errors is not null)
        {
            response["errors"] = errors;
        }

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}

using Beatport2Rss.Api.Endpoints.EmailVerifications;

namespace Beatport2Rss.Api.Middlewares.EmailVerifications;

internal sealed partial class EmailConfirmationPrivacyMiddleware(RequestDelegate next, ILogger<EmailConfirmationPrivacyMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments(EmailConfirmationEndpoints.Route, StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        context.Response.Headers.CacheControl = "no-store";
        context.Response.Headers["Referrer-Policy"] = "no-referrer";

        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Abort();
        }
#pragma warning disable CA1031 // Do not forward exceptions for capability URLs to path-logging middleware.
        catch (Exception exception)
#pragma warning restore CA1031
        {
            LogFailure(exception.GetType().Name, context.TraceIdentifier);
            if (context.Response.HasStarted)
            {
                context.Abort();
                return;
            }

            await Results.Problem(statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(context);
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Email confirmation request failed ({ExceptionType}); trace {TraceId}.")]
    private partial void LogFailure(string exceptionType, string traceId);
}

using Beatport2Rss.Api.Constants;

namespace Beatport2Rss.Api.Middlewares;

internal static class TraceIdMiddleware
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseTraceIdMiddleware() =>
            app.Use(HandleAsync);

        private static async Task HandleAsync(HttpContext context, Func<Task> next)
        {
            context.Response.Headers[ResponseHeaderNames.TraceId] = context.TraceIdentifier;
            await next();
        }
    }
}
using Beatport2Rss.WebApi.Constants;

namespace Beatport2Rss.WebApi.Middlewares;

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
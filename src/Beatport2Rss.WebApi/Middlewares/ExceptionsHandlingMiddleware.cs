#pragma warning disable CA1031 // Do not catch general exception types

using System.Net;

using Beatport2Rss.WebApi.Responses;

namespace Beatport2Rss.WebApi.Middlewares;

internal static class ExceptionsHandlingMiddleware
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder AddExceptionsHandlingMiddleware() => app.Use(HandleAsync);

        private static async Task HandleAsync(HttpContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception exception)
            {
                var response = new InternalServerErrorResponse
                {
                    Title = "Internal server error",
                    Detail = exception.ToString(),
                };

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
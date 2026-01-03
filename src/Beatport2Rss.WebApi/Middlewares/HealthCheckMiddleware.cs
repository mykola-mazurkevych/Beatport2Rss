using System.Net.Mime;

using Beatport2Rss.Infrastructure.Constants;
using Beatport2Rss.WebApi.Responses.Health;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Beatport2Rss.WebApi.Middlewares;

internal static class HealthCheckMiddleware
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseHealthCheckMiddleware() =>
            app.UseHealthChecks(
                "/health",
                new HealthCheckOptions { ResponseWriter = WriteResponseAsync, ResultStatusCodes = ResultStatusCodes });

        private static async Task WriteResponseAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = new HealthResponse
            {
                Status = report.Status,
                Details = new HealthDetailsResponse
                {
                    DatabaseStatus = report.Entries[HealthCheckNames.Database].Status
                }
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private static IDictionary<HealthStatus, int> ResultStatusCodes =>
        new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
            { HealthStatus.Degraded, StatusCodes.Status503ServiceUnavailable },
            { HealthStatus.Healthy, StatusCodes.Status200OK },
        };
}
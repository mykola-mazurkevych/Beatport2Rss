using System.Net.Mime;

using Beatport2Rss.Infrastructure.Constants;
using Beatport2Rss.WebApi.Responses.Health;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class HealthEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildHealthEndpoints()
        {
            routeBuilder.MapHealthChecks("/health/current", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
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
                },
                ResultStatusCodes = new Dictionary<HealthStatus, int>
                {
                    { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
                    { HealthStatus.Degraded, StatusCodes.Status503ServiceUnavailable },
                    { HealthStatus.Healthy, StatusCodes.Status200OK },
                }
            });

            return routeBuilder;
        }
    }
}
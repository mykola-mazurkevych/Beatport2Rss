using System.Net.Mime;

using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Responses.Health;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class HealthEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildHealthEndpoints()
        {
            var groupBuilder = routeBuilder.MapGroup("/health/current").WithName("Health");

            groupBuilder.MapGet(
                    "",
                    async ([FromServices] IDatabaseHealthService databaseHealthService, CancellationToken cancellationToken) =>
                    {
                        var response = new HealthResponse(await databaseHealthService.IsHealthyAsync(cancellationToken));

                        return response.IsHealthy
                            ? Results.Ok(response)
                            : Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
                    })
                .WithName(HealthEndpointNames.Get)
                .WithDescription("Get API health.")
                .AllowAnonymous()
                .Produces<HealthResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)
                .Produces<HealthResponse>(StatusCodes.Status503ServiceUnavailable, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
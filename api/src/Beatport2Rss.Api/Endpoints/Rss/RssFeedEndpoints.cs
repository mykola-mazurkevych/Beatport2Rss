using Beatport2Rss.Api.Endpoints.Rss.Handlers;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Rss;

internal static class RssFeedEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildRssFeedEndpoints()
        {
            routeBuilder
                .MapGet("/feeds/{slug}/rss", GetRssFeedEndpointHandler.Handle)
                .AllowAnonymous()
                .WithName("GetRssFeed")
                .WithTags("RSS")
                .WithDescription("Get the RSS document for a feed")
                .WithSummary("Get RSS Feed")
                .Produces(StatusCodes.Status200OK, contentType: "application/rss+xml")
                .Produces(StatusCodes.Status304NotModified)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json");

            return routeBuilder;
        }
    }
}
using System.Net.Mime;

using Beatport2Rss.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Infrastructure.Extensions;
using Beatport2Rss.WebApi.Constants.Endpoints;
using Beatport2Rss.WebApi.Extensions;
using Beatport2Rss.WebApi.Requests.Feeds;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class FeedEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildFeedEndpoints()
        {
            var groupBuilder = routeBuilder.MapGroup("/feeds").WithName("Feeds");

            groupBuilder
                .MapPost(
                    "",
                    async ([FromBody] CreateFeedRequestBody body, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateFeedCommand(
                            context.User.Id,
                            body.Name);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToIResult(() => Results.CreatedAtRoute(FeedEndpointNames.Get, routeValues: new { slug = result.Value }), context);
                    })
                .WithName(FeedEndpointNames.Create)
                .WithDescription("Create a feed.")
                .RequireAuthorization()
                .Accepts<CreateFeedRequestBody>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet(
                    "/{slug}",
                    async ([FromRoute] string slug, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new GetFeedBySlugQuery(
                            context.User.Id,
                            slug);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToIResult(() => Results.Ok(result.Value), context);
                    })
                .WithName(FeedEndpointNames.Get)
                .WithDescription("Get a feed by slug.")
                .RequireAuthorization()
                .Produces<GetFeedBySlugResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
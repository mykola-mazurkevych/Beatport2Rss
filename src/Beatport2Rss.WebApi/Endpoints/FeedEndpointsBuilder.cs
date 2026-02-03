using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Application.ReadModels.Feeds;
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
        public IEndpointRouteBuilder BuildFeedEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder.MapGroup("/feeds")
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Feeds");

            //// groupBuilder.MapGet("", ...); // Get feeds

            groupBuilder
                .MapPost(
                    "",
                    async ([FromBody] CreateFeedRequestBody body, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateFeedCommand(
                            context.User.Id,
                            body.Name);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(FeedEndpointNames.Get, routeValues: new { slug = result.Value }), context);
                    })
                .WithName(FeedEndpointNames.Create)
                .WithDescription("Create a feed")
                .WithSummary("Create")
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
                        var query = new GetFeedQuery(
                            context.User.Id,
                            slug);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(result.Value), context);
                    })
                .WithName(FeedEndpointNames.Get)
                .WithDescription("Get a feed by slug")
                .WithSummary("Get by Slug")
                .RequireAuthorization()
                .Produces<FeedDetailsReadModel>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            //// groupBuilder.MapPut("/slug", ...); // Update feed

            groupBuilder
                .MapPut(
                    "/{slug}/status",
                    async ([FromRoute] string slug, [FromBody] UpdateFeedStatusRequestBody body, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateFeedStatusCommand(
                            context.User.Id,
                            slug,
                            body.IsActive);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(FeedEndpointNames.UpdateStatus)
                .WithDescription("Update status of a feed by slug")
                .WithSummary("Update Status by Slug")
                .RequireAuthorization()
                .Accepts<UpdateFeedStatusRequestBody>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/{slug}",
                    async ([FromRoute] string slug, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new DeleteFeedCommand(
                            context.User.Id,
                            slug);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(FeedEndpointNames.Delete)
                .WithDescription("Delete a feed by slug")
                .WithSummary("Delete by Slug")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
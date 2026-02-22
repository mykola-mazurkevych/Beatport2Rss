using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.WebApi.Endpoints.Feeds.Requests;
using Beatport2Rss.WebApi.Endpoints.Feeds.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Feeds;

file static class FeedEndpointNames
{
    public const string Create = "CreateFeed";
    public const string Delete = "DeleteFeed";
    public const string Get = "GetFeed";
    public const string UpdateStatus = "UpdateFeedStatus";
}

internal static class FeedEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildFeedEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder.MapGroup("feeds")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Feeds");

            //// groupBuilder.MapGet("", ...); // Get feeds

            groupBuilder
                .MapPost(
                    "",
                    static async ([FromBody] [Required] CreateFeedRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateFeedCommand(
                            context.User.Id,
                            request.Name);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(FeedEndpointNames.Get, routeValues: new { slug = result.Value }), context);
                    })
                .WithName(FeedEndpointNames.Create)
                .WithDescription("Create a new feed")
                .WithSummary("Create")
                .Accepts<CreateFeedRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet(
                    "/{slug}",
                    static async ([FromRoute] Slug slug, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new GetFeedQuery(
                            context.User.Id,
                            slug);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(GetFeedResponse.Create(result.Value)), context);
                    })
                .WithName(FeedEndpointNames.Get)
                .WithDescription("Get feed details by its slug")
                .WithSummary("Get Details")
                .Produces<GetFeedResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            //// groupBuilder.MapPut("/slug", ...); // Update feed

            groupBuilder
                .MapPut(
                    "/{slug}/status",
                    static async ([FromRoute] Slug slug, [FromBody] UpdateFeedStatusRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateFeedStatusCommand(
                            context.User.Id,
                            slug,
                            request.IsActive);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(FeedEndpointNames.UpdateStatus)
                .WithDescription("Update status of a feed by its slug")
                .WithSummary("Update Status")
                .Accepts<UpdateFeedStatusRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/{slug}",
                    static async ([FromRoute] Slug slug, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteFeedCommand(
                            context.User.Id,
                            slug);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(FeedEndpointNames.Delete)
                .WithDescription("Delete a feed by its slug")
                .WithSummary("Delete")
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
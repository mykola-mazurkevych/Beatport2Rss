using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Application.UseCases.Subscriptions.Queries;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Requests;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions;

file static class SubscriptionEndpointNames
{
    public const string CreateArtist = "CreateArtist";
    public const string CreateArtistTag = "CreateArtistTag";
    public const string CreateLabel = "CreateLabel";
    public const string CreateLabelTag = "CreateLabelTag";
    public const string GetArtist = "GetArtist";
    public const string GetLabel = "GetLabel";
}

internal static class SubscriptionEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildSubscriptionEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder.MapGroup("subscriptions")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Subscriptions");

            groupBuilder
                .MapPost(
                    "artists",
                    static async (
                        [FromBody] [Required] CreateSubscriptionRequest request,
                        [FromServices] IMediator mediator,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSubscriptionCommand(
                            BeatportSubscriptionType.Artist,
                            request.BeatportId);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(
                            () => Results.CreatedAtRoute(
                                SubscriptionEndpointNames.GetArtist,
                                new { result.Value.BeatportSlug, result.Value.BeatportId },
                                SubscriptionResponse.Create(result.Value)),
                            context);
                    })
                .WithName(SubscriptionEndpointNames.CreateArtist)
                .WithDescription("Create an artist subscription")
                .WithSummary("Create Artist")
                .Accepts<CreateSubscriptionRequest>(MediaTypeNames.Application.Json)
                .Produces<SubscriptionResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPost(
                    "labels",
                    static async (
                        [FromBody] [Required] CreateSubscriptionRequest request,
                        [FromServices] IMediator mediator,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSubscriptionCommand(
                            BeatportSubscriptionType.Label,
                            request.BeatportId);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(
                            () => Results.CreatedAtRoute(
                                SubscriptionEndpointNames.GetLabel,
                                new { result.Value.BeatportSlug, result.Value.BeatportId },
                                SubscriptionResponse.Create(result.Value)),
                            context);
                    })
                .WithName(SubscriptionEndpointNames.CreateLabel)
                .WithDescription("Create a label subscription")
                .WithSummary("Create Label")
                .Accepts<CreateSubscriptionRequest>(MediaTypeNames.Application.Json)
                .Produces<SubscriptionResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet(
                    "artists/{beatportSlug}/{beatportId}",
                    static async (
                        [FromRoute] BeatportSlug beatportSlug,
                        [FromRoute] BeatportId beatportId,
                        [FromServices] IMediator mediator,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        var query = new GetSubscriptionQuery(
                            context.User.Id,
                            BeatportSubscriptionType.Artist,
                            beatportId,
                            beatportSlug);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(SubscriptionResponse.Create(result.Value)), context);
                    })
                .WithName(SubscriptionEndpointNames.GetArtist)
                .WithDescription("Get an artist by Beatport Slug and Beatport Id")
                .WithSummary("Get Artist")
                .Produces<SubscriptionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet(
                    "labels/{beatportSlug}/{beatportId}",
                    static async (
                        [FromRoute] BeatportSlug beatportSlug,
                        [FromRoute] BeatportId beatportId,
                        [FromServices] IMediator mediator,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        var query = new GetSubscriptionQuery(
                            context.User.Id,
                            BeatportSubscriptionType.Label,
                            beatportId,
                            beatportSlug);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(SubscriptionResponse.Create(result.Value)), context);
                    })
                .WithName(SubscriptionEndpointNames.GetLabel)
                .WithDescription("Get a label by Beatport Slug and Beatport Id")
                .WithSummary("Get Label")
                .Produces<SubscriptionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPost(
                    "artists/{beatportSlug}/{beatportId}/tags",
                    static async (
                        [FromRoute] BeatportSlug beatportSlug,
                        [FromRoute] BeatportId beatportId,
                        [FromBody] [Required] CreateSubscriptionTagRequest request,
                        [FromServices] IMediator mediator,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSubscriptionTagCommand(
                            context.User.Id,
                            BeatportSubscriptionType.Artist,
                            beatportId,
                            beatportSlug,
                            request.TagSlug);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SubscriptionEndpointNames.CreateArtistTag)
                .WithDescription("Add tag to an artist")
                .WithSummary("Create Artist Tag")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            
            groupBuilder
                .MapPost(
                    "labels/{beatportSlug}/{beatportId}/tags",
                    static async (
                        [FromRoute] BeatportSlug beatportSlug,
                        [FromRoute] BeatportId beatportId,
                        [FromBody] [Required] CreateSubscriptionTagRequest request,
                        [FromServices] IMediator mediator,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSubscriptionTagCommand(
                            context.User.Id,
                            BeatportSubscriptionType.Label,
                            beatportId,
                            beatportSlug,
                            request.TagSlug);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SubscriptionEndpointNames.CreateLabelTag)
                .WithDescription("Add tag to a label")
                .WithSummary("Create Label Tag")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
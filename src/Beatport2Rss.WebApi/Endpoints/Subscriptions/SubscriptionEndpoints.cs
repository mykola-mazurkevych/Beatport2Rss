using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.WebApi.Endpoints.Subscriptions.Handlers;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Requests;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions;

internal static class SubscriptionEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildSubscriptionEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder
                .MapGroup("subscriptions")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Subscriptions");

            groupBuilder
                .MapPost("artists", CreateSubscriptionArtistEndpointHandler.Handle)
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
                .MapPost("labels", CreateSubscriptionLabelEndpointHandler.Handle)
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
                .MapGet("artists/{beatportSlug}/{beatportId}", GetSubscriptionArtistEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.GetArtist)
                .WithDescription("Get an artist by Beatport Slug and Beatport Id")
                .WithSummary("Get Artist")
                .Produces<SubscriptionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet("labels/{beatportSlug}/{beatportId}", GetSubscriptionLabelEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.GetLabel)
                .WithDescription("Get a label by Beatport Slug and Beatport Id")
                .WithSummary("Get Label")
                .Produces<SubscriptionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPost("artists/{beatportSlug}/{beatportId}/tags", CreateSubscriptionArtistTagEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.CreateArtistTag)
                .WithDescription("Add tag to an artist")
                .WithSummary("Create Artist Tag")
                .Accepts<CreateSubscriptionTagRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPost("labels/{beatportSlug}/{beatportId}/tags", CreateSubscriptionLabelTagEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.CreateLabelTag)
                .WithDescription("Add tag to a label")
                .WithSummary("Create Label Tag")
                .Accepts<CreateSubscriptionTagRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("artists/{beatportSlug}/{beatportId}/tags", DeleteSubscriptionArtistTagsEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.DeleteArtistTags)
                .WithDescription("Delete all tags from an artist")
                .WithSummary("Delete Artist Tags")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("labels/{beatportSlug}/{beatportId}/tags", DeleteSubscriptionLabelTagsEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.DeleteLabelTags)
                .WithDescription("Delete all tags from an artist")
                .WithSummary("Delete Label Tags")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("artists/{beatportSlug}/{beatportId}/tags/{slug}", DeleteSubscriptionArtistTagEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.DeleteArtistTag)
                .WithDescription("Delete tag from an artist")
                .WithSummary("Delete Artist Tag")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("labels/{beatportSlug}/{beatportId}/tags/{slug}", DeleteSubscriptionLabelTagEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.DeleteLabelTag)
                .WithDescription("Delete tag from a label")
                .WithSummary("Delete Label Tag")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
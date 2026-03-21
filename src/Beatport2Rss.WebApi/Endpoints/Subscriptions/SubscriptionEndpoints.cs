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
                .MapPost("", CreateSubscriptionEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.Create)
                .WithDescription("Create a subscription by type")
                .WithSummary("Create Subscription")
                .Accepts<CreateSubscriptionRequest>(MediaTypeNames.Application.Json)
                .Produces<SubscriptionResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet("{slug}", GetSubscriptionEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.Get)
                .WithDescription("Get a subscription by slug")
                .WithSummary("Get Subscription")
                .Produces<SubscriptionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPost("{slug}/tags", CreateSubscriptionTagEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.CreateTag)
                .WithDescription("Add tag to a subscription")
                .WithSummary("Create Subscription Tag")
                .Accepts<CreateSubscriptionTagRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("{slug}/tags", DeleteSubscriptionTagsEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.DeleteTags)
                .WithDescription("Delete all tags from a subscription")
                .WithSummary("Delete Subscription Tags")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("{slug}/tags/{tagSlug}", DeleteSubscriptionTagEndpointHandler.Handle)
                .WithName(SubscriptionEndpointNames.DeleteTag)
                .WithDescription("Delete tag from a subscription")
                .WithSummary("Delete Subscription Tag")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
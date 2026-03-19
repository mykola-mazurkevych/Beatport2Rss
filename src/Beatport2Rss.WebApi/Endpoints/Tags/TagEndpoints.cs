using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.WebApi.Endpoints.Tags.Handlers;
using Beatport2Rss.WebApi.Endpoints.Tags.Requests;
using Beatport2Rss.WebApi.Endpoints.Tags.Responses;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Tags;

internal static class TagEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildTagEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder
                .MapGroup("tags")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Tags");

            groupBuilder
                .MapGet("", ListTagsEndpointHandler.Handle)
                .WithName(TagEndpointNames.List)
                .WithDescription("Get a list of tags")
                .WithSummary("List")
                .Produces<PageResponse<TagsResponse>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPost("", CreateTagEndpointHandler.Handle)
                .WithName(TagEndpointNames.Create)
                .WithDescription("Create a new tag")
                .WithSummary("Create")
                .Accepts<CreateTagRequest>(MediaTypeNames.Application.Json)
                .Produces<TagResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet("/{slug}", GetTagEndpointHandler.Handle)
                .WithName(TagEndpointNames.Get)
                .WithDescription("Get tag details by its slug")
                .WithSummary("Get Details")
                .Produces<TagResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

        groupBuilder
                .MapPut("/{slug}/name", UpdateTagNameEndpointHandler.Handle)
                .WithName(TagEndpointNames.UpdateName)
                .WithDescription("Update tag's name by its slug")
                .WithSummary("Update Name")
                .Accepts<UpdateTagNameRequest>(MediaTypeNames.Application.Json)
                .Produces<TagResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("/{slug}", DeleteTagEndpointHandler.Handle)
                .WithName(TagEndpointNames.Delete)
                .WithDescription("Delete a tag by its slug")
                .WithSummary("Delete")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
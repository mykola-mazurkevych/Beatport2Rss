using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Application.UseCases.Tags.Commands;
using Beatport2Rss.Application.UseCases.Tags.Queries;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.WebApi.Endpoints.Tags.Requests;
using Beatport2Rss.WebApi.Endpoints.Tags.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Tags;

file static class TagEndpointNames
{
    public const string Create = "CreateTag";
    public const string Delete = "DeleteTag";
    public const string Get = "GetTag";
    public const string UpdateName = "UpdateName";
}

internal static class TagEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildTagEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder.MapGroup("tags")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Tags");

            // groupBuilder.MapGet("", ...) // Get list of tags

            groupBuilder
                .MapPost(
                    "",
                    static async ([FromBody] [Required] CreateTagRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateTagCommand(
                            context.User.Id,
                            request.Name);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(TagEndpointNames.Get, new { slug = result.Value.Slug }, TagResponse.Create(result.Value)), context);
                    })
                .WithName(TagEndpointNames.Create)
                .WithDescription("Create a new tag")
                .WithSummary("Create")
                .Accepts<CreateTagRequest>(MediaTypeNames.Application.Json)
                .Produces<TagResponse>(StatusCodes.Status201Created)
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
                        var query = new GetTagQuery(
                            context.User.Id,
                            slug);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(TagResponse.Create(result.Value)), context);
                    }
                )
                .WithName(TagEndpointNames.Get)
                .WithDescription("Get tag details by its slug")
                .WithSummary("Get Details")
                .Produces<TagResponse>()
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

        groupBuilder
                .MapPut(
                    "/{slug}/name",
                    static async ([FromRoute] Slug slug, [FromBody] [Required] UpdateTagNameRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateTagNameCommand(
                            context.User.Id,
                            slug,
                            request.Name);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(TagResponse.Create(result.Value)), context);
                    })
                .WithName(TagEndpointNames.UpdateName)
                .WithDescription("Update tag's name by its slug")
                .WithSummary("Update Name")
                .Produces<TagResponse>()
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
                        var command = new DeleteTagCommand(
                            context.User.Id,
                            slug);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(TagEndpointNames.Delete)
                .WithDescription("Delete a tag by its slug")
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
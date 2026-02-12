using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Application.UseCases.Tags.Commands;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints;

file static class TagEndpointNames
{
    public const string Create = "CreateTag";
    public const string Delete = "DeleteTag";
    public const string Get = "GetTag";
}

internal static class TagEndpointsBuilder
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

            groupBuilder
                .MapPost(
                    "",
                    async ([FromBody] [Required] CreateTagRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateTagCommand(
                            context.User.Id,
                            request.Name);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(TagEndpointNames.Get, routeValues: new { slug = result.Value }), context);
                    })
                .WithName(TagEndpointNames.Create)
                .WithDescription("Create a new tag")
                .WithSummary("Create")
                .Accepts<CreateTagRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/{slug}",
                    async ([FromRoute] Slug slug, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
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
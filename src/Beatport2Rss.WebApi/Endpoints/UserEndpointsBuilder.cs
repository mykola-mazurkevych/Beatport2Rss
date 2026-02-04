using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.Application.UseCases.Users.Queries;
using Beatport2Rss.WebApi.Extensions;
using Beatport2Rss.WebApi.Requests.Users;
using Beatport2Rss.WebApi.Responses.Users;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints;

file static class UserEndpointNames
{
    public const string Create = "CreateUser";
    public const string Delete = "DeleteUser";
    public const string DeleteCurrent = "DeleteCurrentUser";
    public const string GetCurrent = "GetCurrentUser";
    public const string UpdateCurrentStatus = "UpdateCurrentUserStatus";
}

internal static class UserEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildUserEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder.MapGroup("/users")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Users");

            ////groupBuilder.MapGet("", ...); // Load all users (for admin?)

            groupBuilder
                .MapPost(
                    "",
                    async ([FromBody] CreateUserRequestBody body, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateUserCommand(
                            body.EmailAddress,
                            body.Password,
                            body.FirstName,
                            body.LastName);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.StatusCode(StatusCodes.Status201Created), context);
                    })
                .WithName(UserEndpointNames.Create)
                .WithDescription("Create a new user")
                .WithSummary("Create")
                .AllowAnonymous()
                .Accepts<CreateUserRequestBody>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet(
                    "/current",
                    async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new GetUserQuery(context.User.Id);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(UserDetailsResponse.Create(result.Value)), context);
                    })
                .WithName(UserEndpointNames.GetCurrent)
                .WithDescription("Get current user details")
                .WithSummary("Get Details")
                .Produces<UserDetailsResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            //// groupBuilder.MapPut("/current", ...); // Update current user

            // Temporary
            groupBuilder
                .MapPut(
                    "/current/status",
                    async ([FromBody] UpdateUserStatusRequestBody body, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateUserStatusCommand(
                            context.User.Id,
                            body.IsActive);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(UserEndpointNames.UpdateCurrentStatus)
                .WithDescription("Update current user status (temporary for testing)")
                .WithSummary("Update Status")
                .Accepts<UpdateUserStatusRequestBody>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            //// groupBuilder.MapPut("/{id:guid}", ...); // Update user by id (by admin?)

            groupBuilder
                .MapDelete(
                    "/current",
                    async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new DeleteUserCommand(context.User.Id);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(UserEndpointNames.DeleteCurrent)
                .WithDescription("Delete current user")
                .WithSummary("Delete Current")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/{id:guid}",
                    async ([FromRoute] Guid id, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new DeleteUserCommand(id);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(UserEndpointNames.Delete)
                .WithDescription("Delete a user by its id")
                .WithSummary("Delete")
                .RequireAuthorization(p => p.RequireRole("admin")) // TODO: implement
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
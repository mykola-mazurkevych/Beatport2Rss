using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Application.UseCases.Sessions.Queries;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.WebApi.Endpoints.Sessions.Requests;
using Beatport2Rss.WebApi.Endpoints.Sessions.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Sessions;

file static class SessionEndpointNames
{
    public const string Create = "CreateSession";
    public const string DeleteAll = "DeleteAllSessions";
    public const string DeleteById = "DeleteSession";
    public const string DeleteCurrent = "DeleteCurrentSession";
    public const string GetCurrent = "GetCurrentSession";
    public const string UpdateCurrent = "UpdateCurrentSession";
}

internal static class SessionEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildSessionEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder.MapGroup("/sessions")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Sessions");

            //// groupBuilder.MapGet("", ...); // Get sessions for curent user. Think if it's needed to see all sessions (for admin?) (should be moved to user/current/sessions maybe?)

            groupBuilder
                .MapPost(
                    "",
                    static async ([FromBody] [Required] CreateSessionRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSessionCommand(
                            request.EmailAddress,
                            request.Password,
                            context.Request.Headers.UserAgent,
                            context.Connection.RemoteIpAddress?.ToString());
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(SessionEndpointNames.GetCurrent, value: SessionResponse.Create(result.Value)), context);
                    })
                .AllowAnonymous()
                .WithName(SessionEndpointNames.Create)
                .WithDescription("Create a new user session")
                .WithSummary("Log In")
                .Accepts<SessionResponse>(MediaTypeNames.Application.Json)
                .Produces<SessionResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet(
                    "/current",
                    static async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new GetSessionQuery(
                            context.User.Id,
                            context.User.SessionId);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(result.Value), context);
                    })
                .WithName(SessionEndpointNames.GetCurrent)
                .WithDescription("Get current session details")
                .WithSummary("Get Details")
                .Produces<GetSessionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPut(
                    "/current",
                    static async ([FromBody] [Required] UpdateSessionRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateSessionCommand(
                            context.User.SessionId,
                            request.RefreshToken);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(SessionResponse.Create(result.Value)), context);
                    })
                .WithName(SessionEndpointNames.UpdateCurrent)
                .WithDescription("Update current user session")
                .WithSummary("Refresh Access Token")
                .Accepts<UpdateSessionRequest>(MediaTypeNames.Application.Json)
                .Produces<SessionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/current",
                    static async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionCommand(context.User.SessionId);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SessionEndpointNames.DeleteCurrent)
                .WithDescription("Delete current user session")
                .WithSummary("Log Out")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/{id}",
                    static async ([FromRoute] SessionId id, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionCommand(id);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SessionEndpointNames.DeleteById)
                .WithDescription("Delete a user session by its id (log out from a session)")
                .WithSummary("Delete")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "",
                    static async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionsCommand(context.User.Id);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SessionEndpointNames.DeleteAll)
                .WithDescription("Delete all user sessions (log out from all sessions)")
                .WithSummary("Delete All")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
using System.Net.Mime;

using Beatport2Rss.Application.ReadModels.Sessions;
using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Application.UseCases.Sessions.Queries;
using Beatport2Rss.Infrastructure.Extensions;
using Beatport2Rss.WebApi.Constants.Endpoints;
using Beatport2Rss.WebApi.Extensions;
using Beatport2Rss.WebApi.Requests.Sessions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class SessionEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildSessionEndpoints()
        {
            var groupBuilder = routeBuilder.MapGroup("/sessions").WithName("Sessions");

            //// groupBuilder.MapGet("", ...); // Get sessions for curent user. Think if it's needed to see all sessions (for admin?)

            groupBuilder
                .MapPost(
                    "",
                    async ([FromBody] CreateSessionRequestBody body, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSessionCommand(
                            body.EmailAddress,
                            body.Password,
                            context.Request.Headers.UserAgent,
                            context.Connection.RemoteIpAddress?.ToString());
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(SessionEndpointNames.GetCurrent, value: result.Value), context);
                    })
                .WithName(SessionEndpointNames.Create)
                .WithDescription("Create a user session (log in).")
                .AllowAnonymous()
                .Accepts<CreateSessionRequestBody>(MediaTypeNames.Application.Json)
                .Produces<CreateSessionResult>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet(
                    "/current",
                    async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new GetSessionQuery(
                            context.User.Id,
                            context.User.SessionId);
                        var result = await mediator.Send(query, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(result.Value), context);
                    })
                .WithName(SessionEndpointNames.GetCurrent)
                .WithDescription("Get current session.")
                .RequireAuthorization()
                .Produces<SessionDetailsReadModel>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPut(
                    "/current",
                    async ([FromBody] UpdateSessionRequestBody body, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateSessionCommand(
                            context.User.SessionId,
                            body.RefreshToken);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(() => Results.Ok(result.Value), context);
                    })
                .WithName(SessionEndpointNames.UpdateCurrent)
                .WithDescription("Update current user session (refresh access token).")
                .RequireAuthorization()
                .Accepts<UpdateSessionRequestBody>(MediaTypeNames.Application.Json)
                .Produces<UpdateSessionResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/current",
                    async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionCommand(context.User.SessionId);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SessionEndpointNames.DeleteCurrent)
                .WithDescription("Delete current user session (log out).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "/{id:guid}",
                    async ([FromRoute] Guid id, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionCommand(id);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SessionEndpointNames.DeleteById)
                .WithDescription("Delete a user session.")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete(
                    "",
                    async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionsCommand(context.User.Id);
                        var result = await mediator.Send(command, cancellationToken);
                        return result.ToAspNetCoreResult(Results.NoContent, context);
                    })
                .WithName(SessionEndpointNames.DeleteAll)
                .WithDescription("Delete all user sessions (log out from all devices).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
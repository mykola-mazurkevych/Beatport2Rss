using System.Net.Mime;

using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Application.UseCases.Sessions.Queries;
using Beatport2Rss.Infrastructure.Extensions;
using Beatport2Rss.WebApi.Constants.Endpoints;
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

            groupBuilder
                .MapPost(
                    "",
                    async ([FromBody] CreateSessionRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSessionCommand(
                            request.EmailAddress,
                            request.Password,
                            context.Request.Headers.UserAgent,
                            context.Connection.RemoteIpAddress?.ToString());
                        var response = await mediator.Send(command, cancellationToken);
                        return response.MatchFirst(
                            createSessionResult => Results.CreatedAtRoute(SessionEndpointNames.GetCurrent, value: createSessionResult),
                            e => ProblemDetailsBuilder.Build(context, e));
                    })
                .WithName(SessionEndpointNames.Create)
                .WithDescription("Create a user session (log in).")
                .AllowAnonymous()
                .Accepts<CreateSessionRequest>(MediaTypeNames.Application.Json)
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
                        var query = new GetSessionQuery(context.User.SessionId);
                        var response = await mediator.Send(query, cancellationToken);
                        return response.MatchFirst(
                            Results.Ok,
                            e => ProblemDetailsBuilder.Build(context, e));
                    })
                .WithName(SessionEndpointNames.GetCurrent)
                .WithDescription("Get current session.")
                .RequireAuthorization()
                .Produces<GetSessionResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
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
                        var response = await mediator.Send(command, cancellationToken);
                        return response.MatchFirst(
                            _ => Results.NoContent(),
                            e => ProblemDetailsBuilder.Build(context, e));
                    })
                .WithName(SessionEndpointNames.DeleteCurrent)
                .WithDescription("Delete current user session (log out).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPatch(
                    "/current",
                    async ([FromBody] UpdateSessionRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateSessionCommand(
                            context.User.SessionId,
                            request.RefreshToken);
                        var response = await mediator.Send(command, cancellationToken);
                        return response.MatchFirst(
                            Results.Ok,
                            e => ProblemDetailsBuilder.Build(context, e));
                    })
                .WithName(SessionEndpointNames.UpdateCurrent)
                .WithDescription("Update current user session (refresh access token).")
                .RequireAuthorization()
                .Accepts<UpdateSessionRequest>(MediaTypeNames.Application.Json)
                .Produces<UpdateSessionResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            groupBuilder
                .MapDelete(
                    "",
                    async ([FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionsCommand(context.User.Id);
                        var response = await mediator.Send(command, cancellationToken);
                        return response.MatchFirst(
                            _ => Results.NoContent(),
                            e => ProblemDetailsBuilder.Build(context, e));
                    })
                .WithName(SessionEndpointNames.DeleteAll)
                .WithDescription("Delete all user sessions (log out from all devices).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            return routeBuilder;
        }
    }
}
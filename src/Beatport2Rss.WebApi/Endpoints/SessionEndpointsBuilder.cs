using System.Net.Mime;

using Beatport2Rss.Application.Types;
using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Application.UseCases.Sessions.Queries;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Extensions;
using Beatport2Rss.WebApi.Requests.Sessions;

using Microsoft.AspNetCore.Mvc;

using OneOf;

using Wolverine;

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
                    async ([FromBody] CreateSessionRequest request, [FromServices] IMessageBus bus, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateSessionCommand(
                            request.EmailAddress,
                            request.Password,
                            context.Request.Headers.UserAgent,
                            context.Connection.RemoteIpAddress?.ToString());
                        var result = await bus.InvokeAsync<OneOf<Success<CreateSessionResult>, ValidationFailed, InvalidCredentials, InactiveUser>>(command, cancellationToken);

                        return result.Match<IResult>(
                            success => Results.CreatedAtRoute(SessionEndpointNames.GetCurrent, value: success.Value),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors),
                            invalidCredentials => ProblemDetailsBuilder.Unauthorized(context, invalidCredentials.Detail),
                            inactiveUser => ProblemDetailsBuilder.Forbidden(context, inactiveUser.Detail));
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
                    async ([FromServices] IMessageBus bus, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var query = new GetSessionQuery(
                            context.User.Id,
                            context.User.SessionId);
                        var result = await bus.InvokeAsync<OneOf<Success<GetSessionResult>, ValidationFailed, NotFound, Unprocessable>>(query, cancellationToken);

                        return result.Match<IResult>(
                            success => Results.Ok(success.Value),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors),
                            notFound => ProblemDetailsBuilder.NotFound(context, notFound.Detail),
                            unprocessable => ProblemDetailsBuilder.UnprocessableEntity(context, unprocessable.Detail));
                    })
                .WithName(SessionEndpointNames.GetCurrent)
                .WithDescription("Get current session.")
                .RequireAuthorization()
                .Produces<GetSessionResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("/current", DeleteSessionAsync)
                .WithName(SessionEndpointNames.DeleteCurrent)
                .WithDescription("Delete current user session (log out).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            groupBuilder
                .MapPatch("/current", UpdateSessionAsync)
                .WithName(SessionEndpointNames.UpdateCurrent)
                .WithDescription("Update current user session (refresh access token).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            groupBuilder
                .MapDelete("", DeleteSessionsAsync)
                .WithName(SessionEndpointNames.DeleteAll)
                .WithDescription("Delete all user sessions (log out from all devices).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

            return routeBuilder;
        }

        private static IResult DeleteSessionAsync(HttpContext context) => throw new NotImplementedException();
        private static IResult DeleteSessionsAsync(HttpContext context) => throw new NotImplementedException();
        private static IResult UpdateSessionAsync(HttpContext context) => throw new NotImplementedException();
    }
}
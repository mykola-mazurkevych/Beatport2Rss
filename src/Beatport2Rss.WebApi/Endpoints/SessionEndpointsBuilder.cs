using System.Net.Mime;

using Beatport2Rss.Application.Types;
using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Application.UseCases.Sessions.Queries;
using Beatport2Rss.Infrastructure.Extensions;
using Beatport2Rss.WebApi.Constants;
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
                        var query = new GetSessionQuery(context.User.SessionId);
                        var result = await bus.InvokeAsync<OneOf<Success<GetSessionResult>, ValidationFailed, InactiveUser>>(query, cancellationToken);

                        return result.Match<IResult>(
                            success => Results.Ok(success.Value),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors),
                            inactiveUser => ProblemDetailsBuilder.Forbidden(context, inactiveUser.Detail));
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
                    async ([FromServices] IMessageBus bus, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionCommand(context.User.SessionId);
                        var result = await bus.InvokeAsync<OneOf<Success, ValidationFailed>>(command, cancellationToken);

                        return result.Match<IResult>(
                            _ => Results.NoContent(),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors));
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
                    async ([FromBody] UpdateSessionRequest request, [FromServices] IMessageBus bus, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateSessionCommand(
                            context.User.SessionId,
                            request.RefreshToken);
                        var result = await bus.InvokeAsync<OneOf<Success<UpdateSessionResult>, ValidationFailed, Unauthorized, InactiveUser>>(command, cancellationToken);

                        return result.Match<IResult>(
                            success => Results.Ok(success.Value),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors),
                            unauthorized => ProblemDetailsBuilder.Unauthorized(context, unauthorized.Detail),
                            inactiveUser => ProblemDetailsBuilder.Forbidden(context, inactiveUser.Detail));
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
                    async ([FromServices] IMessageBus bus, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new DeleteSessionsCommand(context.User.Id);
                        var result = await bus.InvokeAsync<OneOf<Success, ValidationFailed>>(command, cancellationToken);

                        return result.Match<IResult>(
                            _ => Results.NoContent(),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors)
                        );
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
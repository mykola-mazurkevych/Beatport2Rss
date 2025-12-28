using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.WebApi.Requests.Sessions;
using Beatport2Rss.WebApi.Responses;

using Microsoft.AspNetCore.Mvc;

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
                .MapPost("", CreateSessionAsync)
                .WithName("CreateSession")
                .WithDescription("Create a user session (log in).")
                .AllowAnonymous()
                .Produces(StatusCodes.Status201Created)
                .Produces<BadRequestResponse>(StatusCodes.Status400BadRequest)
                .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized)
                .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden)
                .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError);

            groupBuilder
                .MapGet("/current", GetSessionAsync)
                .WithName("GetSession")
                .WithDescription("Get current session.")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError);

            groupBuilder
                .MapDelete("/current", DeleteSessionAsync)
                .WithName("DeleteSession")
                .WithDescription("Delete current user session (log out).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError);
            
            groupBuilder
                .MapPatch("/current", UpdateSessionAsync)
                .WithName("UpdateSession")
                .WithDescription("Update current user session (refresh access token).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces<BadRequestResponse>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status422UnprocessableEntity)
                .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError);
            
            groupBuilder
                .MapDelete("", DeleteSessionsAsync)
                .WithName("DeleteSessions")
                .WithDescription("Delete all user sessions (log out from all devices).")
                .RequireAuthorization()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status422UnprocessableEntity)
                .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError);

            return routeBuilder;
        }

        private static async Task<IResult> CreateSessionAsync(
            [FromBody] CreateSessionRequest request,
            [FromServices] IMessageBus bus,
            HttpContext context,
            CancellationToken cancellationToken)
        {
            var command = new CreateSessionCommand(
                request.EmailAddress,
                request.Password,
                context.Request.Headers.UserAgent,
                context.Connection.RemoteIpAddress?.ToString());

            var result = await bus.InvokeAsync<SessionCreatedResult>(command, cancellationToken);

            return Results.CreatedAtRoute("GetSession", result);
        }

        private static IResult DeleteSessionAsync(HttpContext context) => throw new NotImplementedException();
        private static IResult DeleteSessionsAsync(HttpContext context) => throw new NotImplementedException();
        private static IResult GetSessionAsync(HttpContext context) => throw new NotImplementedException();
        private static IResult UpdateSessionAsync(HttpContext context) => throw new NotImplementedException();
    }
}
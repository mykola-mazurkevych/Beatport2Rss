using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.WebApi.Endpoints.Sessions.Handlers;
using Beatport2Rss.WebApi.Endpoints.Sessions.Requests;
using Beatport2Rss.WebApi.Endpoints.Sessions.Responses;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Sessions;

internal static class SessionEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildSessionEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder
                .MapGroup("/sessions")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Sessions");

            //// groupBuilder.MapGet("", ...); // Get sessions for curent user. Think if it's needed to see all sessions (for admin?) (should be moved to user/current/sessions maybe?)

            groupBuilder
                .MapPost("", CreateSessionEndpointHandler.Handle)
                .AllowAnonymous()
                .WithName(SessionEndpointNames.Create)
                .WithDescription("Create a new user session")
                .WithSummary("Log In")
                .Accepts<CreateSessionRequest>(MediaTypeNames.Application.Json)
                .Produces<SessionResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet("/current", GetCurrentSessionEndpointHandler.Handle)
                .WithName(SessionEndpointNames.GetCurrent)
                .WithDescription("Get current session details")
                .WithSummary("Get Details")
                .Produces<SessionResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPut("/current", UpdateCurrentSessionEndpointHandler.Handle)
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
                .MapDelete("/current", DeleteCurrentSessionEndpointHandler.Handle)
                .WithName(SessionEndpointNames.DeleteCurrent)
                .WithDescription("Delete current user session")
                .WithSummary("Log Out")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("/{id}", DeleteSessionEndpointHandler.Handle)
                .WithName(SessionEndpointNames.DeleteById)
                .WithDescription("Delete a user session by its id (log out from a session)")
                .WithSummary("Delete")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapDelete("", DeleteAllSessionsEndpointHandler.Handle)
                .WithName(SessionEndpointNames.DeleteAll)
                .WithDescription("Delete all user sessions (log out from all sessions)")
                .WithSummary("Delete All")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
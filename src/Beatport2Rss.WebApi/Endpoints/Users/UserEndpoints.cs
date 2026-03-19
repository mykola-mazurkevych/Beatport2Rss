using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.WebApi.Endpoints.Users.Handlers;
using Beatport2Rss.WebApi.Endpoints.Users.Requests;
using Beatport2Rss.WebApi.Endpoints.Users.Responses;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Users;

internal static class UserEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildUserEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder
                .MapGroup("/users")
                .RequireAuthorization()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Users");

            ////groupBuilder.MapGet("", ...); // Load all users (for admin?)

            groupBuilder
                .MapPost("", CreateUserEndpointHandler.Handle)
                .WithName(UserEndpointNames.Create)
                .WithDescription("Create a new user")
                .WithSummary("Create")
                .AllowAnonymous()
                .Accepts<CreateUserRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapGet("/current", GetCurrentUserEndpointHandler.Handle)
                .WithName(UserEndpointNames.GetCurrent)
                .WithDescription("Get current user details")
                .WithSummary("Get Details")
                .Produces<UserResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPut("/current", UpdateCurrentUserEndpointHandler.Handle)
                .WithName(UserEndpointNames.UpdateCurrent)
                .WithDescription("Update current user")
                .WithSummary("Update")
                .Accepts<UpdateUserRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPut("/current/email-address", UpdateCurrentUserEmailAddressEndpointHandler.Handle)
                .WithName(UserEndpointNames.UpdateCurrentEmailAddress)
                .WithDescription("Update current user email address")
                .WithSummary("Update Email Address")
                .Accepts<UpdateUserEmailAddressRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPut("/current/password", UpdateCurrentUserPasswordEndpointHandler.Handle)
                .WithName(UserEndpointNames.UpdateCurrentPassword)
                .WithDescription("Update current user password")
                .WithSummary("Update Password")
                .Accepts<UpdateUserPasswordRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            // Temporary
            groupBuilder
                .MapPut("/current/status", UpdateCurrentUserStatusEndpointHandler.Handle)
                .WithName(UserEndpointNames.UpdateCurrentStatus)
                .WithDescription("Update current user status (temporary for testing)")
                .WithSummary("Update Status")
                .Accepts<UpdateUserStatusRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            //// groupBuilder.MapPut("/{id:guid}", ...); // Update user by id (by admin?)

            groupBuilder
                .MapDelete("/current", DeleteCurrentUserEndpointHandler.Handle)
                .WithName(UserEndpointNames.DeleteCurrent)
                .WithDescription("Delete current user")
                .WithSummary("Delete Current")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            // TODO: think if needed
            groupBuilder
                .MapDelete("/{id}", DeleteUserEndpointHandler.Handle)
                .WithName(UserEndpointNames.Delete)
                .WithDescription("Delete a user by its id")
                .WithSummary("Delete")
                .RequireAuthorization(p => p.RequireRole("admin")) // TODO: implement
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
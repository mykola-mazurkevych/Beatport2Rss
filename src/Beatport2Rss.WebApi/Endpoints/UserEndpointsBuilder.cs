using System.Net.Mime;

using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.Application.UseCases.Users.Queries;
using Beatport2Rss.Infrastructure.Extensions;
using Beatport2Rss.WebApi.Constants.Endpoints;
using Beatport2Rss.WebApi.Extensions;
using Beatport2Rss.WebApi.Requests.Users;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class UserEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildUserEndpoints()
        {
            var groupBuilder = routeBuilder.MapGroup("/users").WithName("Users");

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
                .WithDescription("Create a user.")
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
                        return result.ToAspNetCoreResult(() => Results.Ok(result.Value), context);
                    })
                .WithName(UserEndpointNames.GetCurrent)
                .WithDescription("Get current user.")
                .RequireAuthorization()
                .Produces<UserDetailsReadModel>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            //// groupBuilder.MapPut("/current", ...); // Update current user

            //// groupBuilder.MapPut("/{id:guid}", ...); // Update user by id (by admin?)

            //// groupBuilder.MapDelete("/current", ...); // Delete current user
            
            //// groupBuilder.MapDelete("/{id:guid}", ...); // Delete user by id (by admin?)

            return routeBuilder;
        }
    }
}
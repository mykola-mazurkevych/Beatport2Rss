using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.WebApi.Responses;

using Microsoft.AspNetCore.Mvc;

using Wolverine;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class UserEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildUserEndpoints()
        {
            var groupBuilder = routeBuilder.MapGroup("/users").WithName("Users");

            groupBuilder
                .MapPost("", CreateUserAsync)
                .WithName("Create a user.")
                .AllowAnonymous()
                .Produces(StatusCodes.Status201Created)
                .Produces<BadRequestResponse>(StatusCodes.Status400BadRequest)
                .Produces<ConflictResponse>(StatusCodes.Status409Conflict)
                .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError);

            return routeBuilder;
        }
    }

    private static async Task<IResult> CreateUserAsync(
        [FromBody] CreateUserCommand command,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        await bus.InvokeAsync(command, cancellationToken);

        return Results.StatusCode(StatusCodes.Status201Created);
    }
}
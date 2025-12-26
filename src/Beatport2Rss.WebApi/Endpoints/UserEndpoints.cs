using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.WebApi.Responses;

using Microsoft.AspNetCore.Mvc;

using Wolverine;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class UserEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public RouteHandlerBuilder MapUserEndpoints() =>
            builder
                .MapGroup("/users")
                .WithName("Users")
                .MapEndpoints();
    }

    extension(RouteGroupBuilder builder)
    {
        private RouteHandlerBuilder MapEndpoints() =>
            builder
                .MapPost("", CreateUserAsync)
                .WithName("Create a user")
                .AllowAnonymous()
                .Produces(StatusCodes.Status201Created)
                .Produces<BadRequestResponse>(StatusCodes.Status400BadRequest)
                .Produces<ConflictResponse>(StatusCodes.Status409Conflict)
                .Produces<InternalServerErrorResponse>(StatusCodes.Status500InternalServerError);
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
using System.Net.Mime;

using Beatport2Rss.Application.UseCases.Users.Commands;
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
                .Accepts<CreateUserCommand>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
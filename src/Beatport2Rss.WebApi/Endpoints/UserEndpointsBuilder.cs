using System.Net.Mime;

using Beatport2Rss.Application.Results;
using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.WebApi.Constants.Endpoints;

using Microsoft.AspNetCore.Mvc;

using OneOf;

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
                .MapPost(
                    "",
                    async ([FromBody] CreateUserCommand command, [FromServices] IMessageBus bus, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var result = await bus.InvokeAsync<OneOf<Success, ValidationFailed, EmailAddressAlreadyTaken>>(command, cancellationToken);

                        return result.Match<IResult>(
                            _ => Results.StatusCode(StatusCodes.Status201Created),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors),
                            emailAddressAlreadyTaken => ProblemDetailsBuilder.Conflict(context, emailAddressAlreadyTaken.Detail));
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
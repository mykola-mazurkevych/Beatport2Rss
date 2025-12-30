using System.Net.Mime;

using Beatport2Rss.Application.Types;
using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Responses;

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
                .MapPost("", CreateUserAsync)
                .WithName(UserEndpointNames.Create)
                .WithDescription("Create a user.")
                .AllowAnonymous()
                .Accepts<CreateUserCommand>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<BadRequestResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ConflictResponse>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }

    private static Task<IResult> CreateUserAsync(
        [FromBody] CreateUserCommand command,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken) =>
        bus.InvokeAsync<OneOf<Created, ValidationError, EmailAddressAlreadyTaken>>(command, cancellationToken)
            .ContinueWith(t =>
                    t.Result.Match(
                        _ => Results.StatusCode(StatusCodes.Status201Created),
                        CreateBadRequest,
                        CreateConflict),
                TaskScheduler.Current);

    private static IResult CreateBadRequest(ValidationError r) =>
        Results.BadRequest(new BadRequestResponse { Title = "One or more validation errors occurred", Detail = "The request contains invalid data.", Errors = r.Errors });

    private static IResult CreateConflict(EmailAddressAlreadyTaken r) =>
        Results.Conflict(new ConflictResponse { Title = "Email taken", Detail = r.Message });
}
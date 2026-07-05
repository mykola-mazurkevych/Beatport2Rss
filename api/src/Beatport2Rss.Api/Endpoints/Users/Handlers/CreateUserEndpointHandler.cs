using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Endpoints.Users.Requests;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Users.Handlers;

internal static class CreateUserEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] CreateUserRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.EmailAddress,
            request.Password,
            request.FirstName,
            request.LastName,
            request.CountryCode);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.StatusCode(StatusCodes.Status201Created), context);
    }
}
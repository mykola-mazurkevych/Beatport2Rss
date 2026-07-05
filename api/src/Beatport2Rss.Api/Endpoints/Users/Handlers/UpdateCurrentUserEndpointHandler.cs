using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Endpoints.Users.Requests;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Users.Handlers;

internal static class UpdateCurrentUserEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] UpdateUserRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            context.User.Id,
            request.FirstName,
            request.LastName,
            request.CountryCode);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
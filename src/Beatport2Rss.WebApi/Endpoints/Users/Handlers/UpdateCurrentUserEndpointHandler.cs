using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.WebApi.Endpoints.Users.Requests;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Users.Handlers;

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
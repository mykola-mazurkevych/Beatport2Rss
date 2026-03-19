using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.WebApi.Endpoints.Users.Requests;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Users.Handlers;

internal static class UpdateCurrentUserPasswordEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] UpdateUserPasswordRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserPasswordCommand(
            context.User.Id,
            request.Password);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
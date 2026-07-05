using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Endpoints.Users.Requests;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Users.Handlers;

internal static class UpdateCurrentUserEmailAddressEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] UpdateUserEmailAddressRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserEmailAddressCommand(
            context.User.Id,
            request.EmailAddress);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
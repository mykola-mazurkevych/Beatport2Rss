using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Users.Handlers;

internal static class DeleteUserEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] UserId id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
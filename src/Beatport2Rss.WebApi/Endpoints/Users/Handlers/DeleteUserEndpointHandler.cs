using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Users.Handlers;

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
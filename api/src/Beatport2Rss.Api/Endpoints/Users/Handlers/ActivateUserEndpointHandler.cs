using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Extensions;
using Beatport2Rss.Common.Domain.Users;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Users.Handlers;

internal static class ActivateUserEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] UserId id,
        [FromServices] IMediator mediator,
        [FromServices] IUserQueryRepository users,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var callerId = context.User.Id;
        if (callerId == id || !await users.ExistsAsync(callerId, cancellationToken))
        {
            return Results.Forbid();
        }

        var command = new UpdateUserStatusCommand(id.Value, IsActive: true);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}

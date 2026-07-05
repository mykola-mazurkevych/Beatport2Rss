using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Users.Handlers;

internal static class DeleteCurrentUserEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(context.User.Id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
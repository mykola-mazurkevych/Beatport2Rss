using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Sessions.Handlers;

internal static class DeleteSessionEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] SessionId id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSessionCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
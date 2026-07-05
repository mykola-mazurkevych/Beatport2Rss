using Beatport2Rss.Api.Application.UseCases.Sessions.Queries;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Sessions.Handlers;

internal static class GetCurrentSessionEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetSessionQuery(
            context.User.Id,
            context.User.SessionId);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(result.Value), context);
    }
}
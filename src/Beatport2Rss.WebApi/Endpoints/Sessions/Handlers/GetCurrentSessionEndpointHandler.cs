using Beatport2Rss.Application.UseCases.Sessions.Queries;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Sessions.Handlers;

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
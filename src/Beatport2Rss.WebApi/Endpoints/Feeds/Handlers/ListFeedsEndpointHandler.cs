using Beatport2Rss.Application.UseCases.Feeds.Queries;
using Beatport2Rss.WebApi.Endpoints.Feeds.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Handlers;

internal static class ListFeedsEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] int? size,
        [FromQuery] string? next,
        [FromQuery] string? previos,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetFeedsQuery(
            context.User.Id,
            size,
            next,
            previos);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(PageResponse<FeedsResponse>.Create(result.Value, FeedsResponse.Create)), context);
    }
}
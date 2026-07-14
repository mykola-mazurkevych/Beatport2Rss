using Beatport2Rss.Api.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Api.Endpoints.Feeds.Responses;
using Beatport2Rss.Api.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Feeds.Handlers;

internal static class GetFeedEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetFeedQuery(
            context.User.Id,
            slug);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(FeedResponse.Create(result.Value)), context);
    }
}
using Beatport2Rss.Application.UseCases.Subscriptions.Queries;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Handlers;

internal static class GetSubscriptionArtistEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] BeatportSlug beatportSlug,
        [FromRoute] BeatportId beatportId,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetSubscriptionQuery(
            context.User.Id,
            BeatportSubscriptionType.Artist,
            beatportId,
            beatportSlug);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(SubscriptionResponse.Create(result.Value)), context);
    }
}
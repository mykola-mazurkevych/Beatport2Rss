using Beatport2Rss.Application.UseCases.Subscriptions.Queries;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Handlers;

internal static class GetSubscriptionEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetSubscriptionQuery(
            context.User.Id,
            slug);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(SubscriptionResponse.Create(result.Value)), context);
    }
}
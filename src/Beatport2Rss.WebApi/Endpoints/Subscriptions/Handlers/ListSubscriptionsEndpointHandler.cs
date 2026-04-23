using Beatport2Rss.Application.UseCases.Subscriptions.Queries;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Handlers;

internal static class ListSubscriptionsEndpointHandler
{
    public static async Task<IResult> Handle(
        [AsParameters] PaginationRequest pageNavigationRequest,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new ListSubscriptionsQuery(
            context.User.Id,
            pageNavigationRequest.ToPagination());
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(
            page =>
            {
                page.Info.ToHeaders(context);
                return Results.Ok(page.Dtos.Select(SubscriptionPaginableResponse.Create));
            },
            context);
    }
}
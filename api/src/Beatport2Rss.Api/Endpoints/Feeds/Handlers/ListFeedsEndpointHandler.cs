using System.Text.Json;

using Beatport2Rss.Api.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Api.Endpoints.Feeds.Responses;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Endpoints.Feeds.Handlers;

internal static class ListFeedsEndpointHandler
{
    public static async Task<IResult> Handle(
        [AsParameters] PaginationRequest pageNavigationRequest,
        [FromServices] IMediator mediator,
        [FromServices] IOptionsSnapshot<JsonSerializerOptions> jsonSerializerOptionsSnapshot,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new ListFeedsQuery(
            context.User.Id,
            pageNavigationRequest.ToPagination());
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(
            page =>
            {
                page.Info.ToHeaders(context);
                return Results.Ok(page.Dtos.Select(FeedPaginableResponse.Create));
            },
            context);
    }
}
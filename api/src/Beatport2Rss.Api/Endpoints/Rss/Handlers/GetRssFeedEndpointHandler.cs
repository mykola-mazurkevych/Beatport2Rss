using System.Text;

using Beatport2Rss.Api.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Api.Extensions;
using Beatport2Rss.Common.Domain.Common;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Rss.Handlers;

internal static class GetRssFeedEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var ifModifiedSince = context.Request.GetTypedHeaders().IfModifiedSince;
        var query = new GetRssFeedQuery(slug, ifModifiedSince);
        var result = await mediator.Send(query, cancellationToken);

        return result.ToAspNetCoreResult(
            response => Results.Content(response.Content, "application/rss+xml", Encoding.UTF8),
            context);
    }
}
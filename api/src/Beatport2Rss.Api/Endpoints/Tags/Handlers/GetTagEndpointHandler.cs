using Beatport2Rss.Api.Application.UseCases.Tags.Queries;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Endpoints.Tags.Responses;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Tags.Handlers;

internal static class GetTagEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetTagQuery(
            context.User.Id,
            slug);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(TagResponse.Create(result.Value)), context);
    }
}
using Beatport2Rss.Application.UseCases.Tags.Queries;
using Beatport2Rss.WebApi.Endpoints.Tags.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Handlers;

internal static class ListTagsEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] int? size,
        [FromQuery] string? next,
        [FromQuery] string? previos,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new ListTagsQuery(
            context.User.Id,
            size,
            next,
            previos);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(PageResponse<TagsResponse>.Create(result.Value, TagsResponse.Create)), context);
    }
}
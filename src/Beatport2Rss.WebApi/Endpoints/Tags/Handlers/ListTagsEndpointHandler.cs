using System.Text.Json;

using Beatport2Rss.Application.UseCases.Tags.Queries;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Endpoints.Tags.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Handlers;

internal static class ListTagsEndpointHandler
{
    public static async Task<IResult> Handle(
        [AsParameters] PageNavigationRequest pageNavigationRequest,
        [FromServices] IMediator mediator,
        [FromServices] IOptionsSnapshot<JsonSerializerOptions> jsonSerializerOptionsSnapshot,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new ListTagsQuery(
            context.User.Id,
            pageNavigationRequest.ToPageNavigation());
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(
            page =>
            {
                var jsonSerializerOptions = jsonSerializerOptionsSnapshot.Get("Plain"); // TODO: use const
                context.Response.Headers[ResponseHeaderNames.PageMetadata] = JsonSerializer.Serialize(page.Metadata, jsonSerializerOptions);
                return Results.Ok(page.Dtos.Select(TagPageResponse.Create));
            },
            context);
    }
}
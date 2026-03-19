using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.WebApi.Endpoints.Feeds.Requests;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Handlers;

internal static class UpdateFeedEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromBody] [Required] UpdateFeedRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new UpdateFeedCommand(
            context.User.Id,
            slug,
            request.Name,
            request.UpdateSlug,
            request.IsActive);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.RedirectToRoute(FeedEndpointNames.Get, routeValues: new { slug = result.Value }), context);
    }
}
using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Api.Endpoints.Feeds.Requests;
using Beatport2Rss.Api.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Feeds.Handlers;

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
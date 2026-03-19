using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Feeds.Commands;
using Beatport2Rss.WebApi.Endpoints.Feeds.Requests;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Handlers;

internal static class CreateFeedEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] CreateFeedRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new CreateFeedCommand(
            context.User.Id,
            request.Name,
            request.IsActive);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(FeedEndpointNames.Get, routeValues: new { slug = result.Value }), context);
    }
}
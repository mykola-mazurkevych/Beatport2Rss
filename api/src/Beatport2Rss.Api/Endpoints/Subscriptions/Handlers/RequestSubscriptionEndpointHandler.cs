using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Api.Endpoints.Subscriptions.Requests;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Handlers;

internal static class RequestSubscriptionEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] RequestSubscriptionRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new RequestSubscriptionCommand(
            request.Type,
            request.BeatportId);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(
            () => Results.Accepted(),
            context);
    }
}
using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Api.Endpoints.Subscriptions.Requests;
using Beatport2Rss.Api.Endpoints.Subscriptions.Responses;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Handlers;

internal static class CreateSubscriptionEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] CreateSubscriptionRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new CreateSubscriptionCommand(
            request.Type,
            request.BeatportId,
            request.CountryCode);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(
            () => Results.CreatedAtRoute(
                SubscriptionEndpointNames.Get,
                new { slug = result.Value.Slug },
                SubscriptionResponse.Create(result.Value)),
            context);
    }
}
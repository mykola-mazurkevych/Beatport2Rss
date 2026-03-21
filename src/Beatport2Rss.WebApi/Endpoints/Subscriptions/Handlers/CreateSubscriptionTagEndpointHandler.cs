using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.WebApi.Endpoints.Subscriptions.Requests;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Handlers;

internal static class CreateSubscriptionTagEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromBody] [Required] CreateSubscriptionTagRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new CreateSubscriptionTagCommand(
            context.User.Id,
            slug,
            request.TagSlug);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Api.Endpoints.Subscriptions.Requests;
using Beatport2Rss.Api.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Handlers;

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
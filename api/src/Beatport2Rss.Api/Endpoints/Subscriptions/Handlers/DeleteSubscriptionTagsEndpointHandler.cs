using Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Handlers;

internal static class DeleteSubscriptionTagsEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSubscriptionTagsCommand(
            context.User.Id,
            slug);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
using Beatport2Rss.Api.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Api.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Feeds.Handlers;

internal sealed class DeleteFeedEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new DeleteFeedCommand(
            context.User.Id,
            slug);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(Results.NoContent, context);
    }
}
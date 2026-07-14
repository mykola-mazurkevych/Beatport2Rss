using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Tags.Commands;
using Beatport2Rss.Api.Endpoints.Tags.Requests;
using Beatport2Rss.Api.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Tags.Handlers;

internal static class UpdateTagNameEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Slug slug,
        [FromBody] [Required] UpdateTagNameRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagNameCommand(
            context.User.Id,
            slug,
            request.Name);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.RedirectToRoute(TagEndpointNames.Get, routeValues: new { slug = result.Value }), context);
    }
}
using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Tags.Commands;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.WebApi.Endpoints.Tags.Requests;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Handlers;

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
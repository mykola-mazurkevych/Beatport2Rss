using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Tags.Commands;
using Beatport2Rss.WebApi.Endpoints.Tags.Requests;
using Beatport2Rss.WebApi.Endpoints.Tags.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Handlers;

internal static class CreateTagEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] CreateTagRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand(
            context.User.Id,
            request.Name);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(TagEndpointNames.Get, new { slug = result.Value.Slug }, TagResponse.Create(result.Value)), context);
    }
}
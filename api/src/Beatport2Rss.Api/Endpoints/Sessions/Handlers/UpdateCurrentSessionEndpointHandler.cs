using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Api.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Api.Endpoints.Sessions.Requests;
using Beatport2Rss.Api.Endpoints.Sessions.Responses;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.Sessions.Handlers;

internal static class UpdateCurrentSessionEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] UpdateSessionRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSessionCommand(
            context.User.SessionId,
            request.RefreshToken);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(SessionResponse.Create(result.Value)), context);
    }
}
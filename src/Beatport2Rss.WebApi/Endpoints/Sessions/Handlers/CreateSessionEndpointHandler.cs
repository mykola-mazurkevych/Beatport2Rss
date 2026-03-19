using System.ComponentModel.DataAnnotations;

using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.WebApi.Endpoints.Sessions.Requests;
using Beatport2Rss.WebApi.Endpoints.Sessions.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Sessions.Handlers;

internal static class CreateSessionEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] [Required] CreateSessionRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var command = new CreateSessionCommand(
            request.EmailAddress,
            request.Password,
            context.Request.Headers.UserAgent,
            context.Connection.RemoteIpAddress?.ToString());
        var result = await mediator.Send(command, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.CreatedAtRoute(SessionEndpointNames.GetCurrent, value: SessionResponse.Create(result.Value)), context);
    }
}
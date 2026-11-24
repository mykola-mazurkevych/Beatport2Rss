using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Endpoints.EmailConfirmations.Responses;
using Beatport2Rss.Api.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.EmailConfirmations.Handlers;

internal static class GetEmailConfirmationEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] EmailConfirmationId id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetEmailConfirmationQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(EmailConfirmationResponse.Create(result.Value)), context);
    }
}
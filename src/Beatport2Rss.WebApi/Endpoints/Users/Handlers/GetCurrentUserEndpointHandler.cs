using Beatport2Rss.Application.UseCases.Users.Queries;
using Beatport2Rss.WebApi.Endpoints.Users.Responses;
using Beatport2Rss.WebApi.Extensions;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.WebApi.Endpoints.Users.Handlers;

internal static class GetCurrentUserEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(context.User.Id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToAspNetCoreResult(() => Results.Ok(UserResponse.Create(result.Value)), context);
    }
}
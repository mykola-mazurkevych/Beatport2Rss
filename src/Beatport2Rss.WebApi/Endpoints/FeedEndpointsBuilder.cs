using System.Net.Mime;

using Beatport2Rss.Application.Results;
using Beatport2Rss.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Infrastructure.Extensions;
using Beatport2Rss.WebApi.Constants.Endpoints;
using Beatport2Rss.WebApi.Requests.Feeds;

using Microsoft.AspNetCore.Mvc;

using OneOf;

using Wolverine;

namespace Beatport2Rss.WebApi.Endpoints;

internal static class FeedEndpointsBuilder
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildFeedEndpoints()
        {
            var groupBuilder = routeBuilder.MapGroup("/feeds").WithName("Feeds");

            groupBuilder
                .MapPost(
                    "",
                    async ([FromBody] CreateFeedRequest request, [FromServices] IMessageBus bus, HttpContext context, CancellationToken cancellationToken) =>
                    {
                        var command = new CreateFeedCommand(
                            context.User.Id,
                            request.Name);
                        var result = await bus.InvokeAsync<OneOf<Success<Guid>, ValidationFailed, InactiveUser, Conflict>>(command, cancellationToken);

                        return result.Match<IResult>(
                            success => Results.CreatedAtRoute(FeedEndpointNames.Get, routeValues: new { feedId = success.Value }),
                            validationFailed => ProblemDetailsBuilder.BadRequest(context, validationFailed.Errors),
                            inactiveUser => ProblemDetailsBuilder.Forbidden(context, inactiveUser.Detail),
                            conflict => ProblemDetailsBuilder.Conflict(context, conflict.Detail));
                    })
                .WithName(FeedEndpointNames.Create)
                .WithDescription("Create a feed.")
                .RequireAuthorization()
                .Accepts<CreateFeedRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status201Created)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}
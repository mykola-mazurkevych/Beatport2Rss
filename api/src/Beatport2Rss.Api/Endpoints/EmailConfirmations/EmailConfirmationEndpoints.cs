using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

using Asp.Versioning.Builder;

using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Endpoints.EmailConfirmations.Handlers;
using Beatport2Rss.Api.Endpoints.EmailConfirmations.Responses;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.EmailConfirmations;

internal static class EmailConfirmationEndpoints
{
    public const string Route = "/email-confirmations";
    private const string IdentifierRoute = "/{id}";
    public const string ResourceRoute = Route + IdentifierRoute;

    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildEmailConfirmationEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder
                .MapGroup(Route)
                .AllowAnonymous()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Email Confirmations");

            groupBuilder
                .MapGet(IdentifierRoute, GetEmailConfirmationEndpointHandler.Handle)
                .WithName(EmailConfirmationEndpointNames.Get)
                .WithDescription("Get email confirmation by its id")
                .WithSummary("Get Details")
                .Produces<EmailConfirmationResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            groupBuilder
                .MapPut(IdentifierRoute, PutAsync)
                .WithDescription("Confirm only with confirmed:true. Repeating a completed confirmation returns 204 without changing account status.")
                .Accepts<ConfirmEmailRequest>(MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            return routeBuilder;
        }
    }

    private static async Task<IResult> PutAsync(string id, HttpRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        ConfirmEmailRequest? body = null;
        if (request.HasJsonContentType())
        {
            try
            {
                body = await request.ReadFromJsonAsync<ConfirmEmailRequest>(cancellationToken);
            }
            catch (JsonException)
            {
                return Invalid(EmailConfirmationErrors.ConfirmationRequired);
            }
            catch (BadHttpRequestException)
            {
                return Invalid(EmailConfirmationErrors.ConfirmationRequired);
            }
        }

        var parsedId = EmailConfirmationId.TryParse(id, null, out var confirmationId) ? confirmationId : (EmailConfirmationId?)null;
        var result = await mediator.Send(new ConfirmEmailCommand(parsedId, body?.Confirmed), cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Invalid(result.Errors[0].Message);
    }

    private static IResult Invalid(string detail) =>
        Results.Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest);

    internal sealed record ConfirmEmailRequest([property: JsonRequired] bool Confirmed);
}

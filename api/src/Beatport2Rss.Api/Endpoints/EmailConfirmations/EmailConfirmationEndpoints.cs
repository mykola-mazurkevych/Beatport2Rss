using System.Text.Json;
using System.Text.Json.Serialization;

using Asp.Versioning.Builder;

using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

using Mediator;

namespace Beatport2Rss.Api.Endpoints.EmailConfirmations;

internal static class EmailConfirmationEndpoints
{
    public const string Route = "/api/email-confirmations";
    private const string IdentifierRoute = "/{id}";
    public const string ResourceRoute = Route + IdentifierRoute;

    public static void BuildEmailConfirmationEndpoints(this IEndpointRouteBuilder builder, ApiVersionSet versionSet)
    {
        var group = builder.MapGroup(Route)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(ApiVersionsContainer.V1)
            .WithTags("Email confirmations")
            .AllowAnonymous();

        group.MapGet(IdentifierRoute, GetAsync)
            .WithDescription("Anonymous. Read confirmation by secret identifier without changing account state. Invalid or expired unused links return 400.")
            .Produces<EmailConfirmationState>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut(IdentifierRoute, PutAsync)
            .WithDescription("Anonymous. Confirm only with confirmed:true. Repeating a completed confirmation returns 204 without changing account status, even after expiry. Invalid or expired unused links return 400.")
            .Accepts<ConfirmEmailRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAsync(string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEmailConfirmationQuery(EmailConfirmationIdentifier.Hash(id)), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : Invalid(result.Errors[0].Message);
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

        var result = await mediator.Send(new ConfirmEmailCommand(EmailConfirmationIdentifier.Hash(id), body?.Confirmed), cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Invalid(result.Errors[0].Message);
    }

    private static IResult Invalid(string detail) =>
        Results.Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest);

    internal sealed record ConfirmEmailRequest([property: JsonRequired] bool Confirmed);
}

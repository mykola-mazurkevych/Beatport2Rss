using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;

public sealed record EmailConfirmationState(bool Confirmed, DateTimeOffset ExpiresAt);

public sealed record GetEmailConfirmationQuery(EmailConfirmationId? Id) : IQuery<Result<EmailConfirmationState>>;

internal sealed class GetEmailConfirmationQueryHandler(
    IEmailConfirmationQueryRepository repository,
    IClock clock,
    IOptions<EmailConfirmationOptions> options) :
    IQueryHandler<GetEmailConfirmationQuery, Result<EmailConfirmationState>>
{
    public async ValueTask<Result<EmailConfirmationState>> Handle(
        GetEmailConfirmationQuery query, CancellationToken cancellationToken)
    {
        var confirmation = options.Value.Enabled && query.Id is { } id
            ? await repository.FindAsync(id, cancellationToken)
            : null;

        return confirmation is not null && confirmation.IsReadableAt(clock.UtcNow)
            ? Result.Ok(new EmailConfirmationState(confirmation.ConfirmedAt is not null, confirmation.ExpiresAt))
            : EmailConfirmationErrors.InvalidLinkResult().ToResult<EmailConfirmationState>();
    }
}

public static class EmailConfirmationErrors
{
    public const string InvalidLink = "This confirmation link is invalid or has expired.";
    public const string ConfirmationRequired = "The confirmed field must be true.";

    internal static Result InvalidLinkResult() => Result.Validation(InvalidLink, []);
    internal static Result ConfirmationRequiredResult() => Result.Validation(ConfirmationRequired, []);
}

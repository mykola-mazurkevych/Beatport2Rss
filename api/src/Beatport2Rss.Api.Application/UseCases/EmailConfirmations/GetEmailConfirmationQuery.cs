using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public sealed record EmailConfirmationState(bool Confirmed, DateTimeOffset ExpiresAt);

public sealed record GetEmailConfirmationQuery(string? Hash) : IQuery<Result<EmailConfirmationState>>;

internal sealed class GetEmailConfirmationQueryHandler(
    IEmailVerificationRepository repository,
    IClock clock,
    IOptions<EmailConfirmationOptions> options) :
    IQueryHandler<GetEmailConfirmationQuery, Result<EmailConfirmationState>>
{
    public async ValueTask<Result<EmailConfirmationState>> Handle(
        GetEmailConfirmationQuery query, CancellationToken cancellationToken)
    {
        var verification = options.Value.Enabled && query.Hash is { } hash
            ? await repository.FindAsync(hash, cancellationToken)
            : null;

        return verification is not null && verification.IsReadableAt(clock.UtcNow)
            ? Result.Ok(new EmailConfirmationState(verification.ConfirmedAt is not null, verification.ExpiresAt))
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

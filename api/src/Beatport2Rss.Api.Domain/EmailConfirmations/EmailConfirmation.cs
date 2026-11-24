using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Domain.EmailConfirmations;

public sealed class EmailConfirmation :
    IAggregateRoot<EmailConfirmationId>
{
    private EmailConfirmation()
    {
    }

    public EmailConfirmationId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public UserId UserId { get; private set; }
    public EmailAddress EmailAddress { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }

    public static EmailConfirmation Create(
        EmailConfirmationId id,
        DateTimeOffset createdAt,
        UserId userId,
        EmailAddress emailAddress,
        DateTimeOffset expiresAt) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            UserId = userId,
            EmailAddress = emailAddress,
            ExpiresAt = expiresAt,
        };

    public void Confirm(DateTimeOffset confirmedAt) =>
        ConfirmedAt = confirmedAt;
}
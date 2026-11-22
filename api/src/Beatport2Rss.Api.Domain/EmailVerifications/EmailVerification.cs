using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Domain.EmailVerifications;

public sealed class EmailVerification :
    IAggregateRoot<EmailVerificationId>
{
    private EmailVerification()
    {
    }

    public EmailVerificationId Id { get; private set; }
    public UserId UserId { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public EmailVerificationSecretHash SecretHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }

    public static EmailVerification Create(
        EmailVerificationId id,
        User user,
        EmailVerificationSecretHash secretHash,
        DateTimeOffset expiresAt) =>
        new()
        {
            Id = id,
            UserId = user.Id,
            EmailAddress = user.EmailAddress,
            SecretHash = secretHash,
            CreatedAt = user.CreatedAt,
            ExpiresAt = expiresAt,
        };

    public bool AppliesTo(User user) =>
        UserId == user.Id && string.Equals(EmailAddress.Value, user.EmailAddress.Value, StringComparison.Ordinal);

    public bool IsReadableAt(DateTimeOffset now) => ConfirmedAt is not null || now < ExpiresAt;

    public bool TryConfirm(User user, DateTimeOffset now)
    {
        if (!AppliesTo(user) || !IsReadableAt(now))
        {
            return false;
        }

        if (ConfirmedAt is null)
        {
            ConfirmedAt = now;
            user.UpdateStatus(isActive: true);
        }

        return true;
    }
}

using Beatport2Rss.Domain.Common.Interfaces;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Domain.Sessions;

public sealed class Session : IAggregateRoot<SessionId>
{
    public const int UserAgentMaxLength = 1024;
    public const int IpAddressMaxLength = 45;

    private Session()
    {
    }

    public SessionId Id { get; private set; }

    public UserId UserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    
    public RefreshTokenHash RefreshTokenHash { get; private set; }
    public DateTimeOffset RefreshTokenExpiresAt { get; private set; }

    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }

    public static Session Create(
        SessionId id,
        UserId userId,
        RefreshTokenHash refreshTokenHash,
        DateTimeOffset refreshTokenExpiresAt,
        string? userAgent,
        string? ipAddress) =>
        new()
        {
            Id = id,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            RefreshTokenHash = refreshTokenHash,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            UserAgent = userAgent,
            IpAddress = ipAddress,
        };

    public void Refresh(RefreshTokenHash refreshTokenHash, DateTimeOffset refreshTokenExpiresAt)
    {
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;
    }
}
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Tokens;

public sealed class Token : IAggregateRoot<TokenId>
{
    private Token()
    {
    }

    public TokenId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportAccessToken AccessToken { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public static Token Create(
        TokenId id,
        DateTimeOffset createdAt,
        BeatportAccessToken accessToken,
        DateTimeOffset expiresAt) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
        };
}
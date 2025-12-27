using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Tokens;

public sealed class Token : IAggregateRoot<TokenId>
{
    private Token()
    {
    }

    public TokenId Id { get; private set; }

    public BeatportAccessToken AccessToken { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }
    public DateTimeOffset ExpirationDate { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpirationDate;

    public static Token Create(
        TokenId id,
        BeatportAccessToken accessToken,
        DateTimeOffset createdDate,
        DateTimeOffset expirationDate) =>
        new()
        {
            Id = id,
            AccessToken = accessToken,
            CreatedDate = createdDate,
            ExpirationDate = expirationDate,
        };
}
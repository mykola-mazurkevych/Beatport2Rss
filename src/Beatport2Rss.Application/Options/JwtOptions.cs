namespace Beatport2Rss.Application.Options;

public sealed record JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public required int ExpiresIn { get; init; }
}
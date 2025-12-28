namespace Beatport2Rss.Application.Options;

public sealed record JwtOptions(
    string Issuer,
    string Audience,
    string SigningKey,
    int ExpiresIn);
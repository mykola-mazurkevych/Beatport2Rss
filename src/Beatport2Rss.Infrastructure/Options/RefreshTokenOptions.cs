namespace Beatport2Rss.Infrastructure.Options;

public sealed record RefreshTokenOptions
{
    public required int ExpiresIn { get; init; }
}
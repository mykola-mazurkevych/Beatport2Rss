namespace Beatport2Rss.Application.Options;

public sealed record RefreshTokenOptions
{
    public required int ExpiresIn { get; init; }
}
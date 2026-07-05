namespace Beatport2Rss.Api.Infrastructure.Options;

public sealed record RefreshTokenOptions
{
    public required int ExpiresIn { get; init; }
}
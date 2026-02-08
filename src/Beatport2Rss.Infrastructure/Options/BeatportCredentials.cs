namespace Beatport2Rss.Infrastructure.Options;

public sealed record BeatportCredentials
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
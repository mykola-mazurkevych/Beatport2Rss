namespace Beatport2Rss.TokenInterceptor.Options;

internal sealed record BeatportCredentials
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
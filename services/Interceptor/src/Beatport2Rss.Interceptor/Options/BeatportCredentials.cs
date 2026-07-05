namespace Beatport2Rss.Interceptor.Options;

internal sealed record BeatportCredentials
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
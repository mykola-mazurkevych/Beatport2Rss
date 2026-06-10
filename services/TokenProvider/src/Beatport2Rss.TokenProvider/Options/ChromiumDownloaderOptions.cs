namespace Beatport2Rss.TokenProvider.Options;

internal sealed record ChromiumDownloaderOptions
{
    public required Uri BaseAddress { get; init; }
}
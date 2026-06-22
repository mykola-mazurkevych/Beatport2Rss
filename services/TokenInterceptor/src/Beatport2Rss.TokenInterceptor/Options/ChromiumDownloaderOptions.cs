namespace Beatport2Rss.TokenInterceptor.Options;

internal sealed record ChromiumDownloaderOptions
{
    public required Uri BaseAddress { get; init; }
}
namespace Beatport2Rss.Interceptor.Options;

internal sealed record ChromiumDownloaderOptions
{
    public required Uri BaseAddress { get; init; }
}
namespace Beatport2Rss.TokenProvider.Options;

internal sealed record ChromiumDownloaderOptions(
    Uri BaseAddress,
    string? CacheDirectory = null);
namespace Beatport2Rss.Interceptor.Interfaces;

internal interface IChromiumDownloader
{
    Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default);
}
namespace Beatport2Rss.TokenInterceptor.Interfaces;

internal interface IChromiumDownloader
{
    Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default);
}
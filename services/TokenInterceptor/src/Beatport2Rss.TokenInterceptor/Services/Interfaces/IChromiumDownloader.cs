namespace Beatport2Rss.TokenInterceptor.Services.Interfaces;

internal interface IChromiumDownloader
{
    Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default);
}
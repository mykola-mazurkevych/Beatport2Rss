namespace Beatport2Rss.TokenProvider.Services.Interfaces;

internal interface IChromiumDownloader
{
    Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default);
}
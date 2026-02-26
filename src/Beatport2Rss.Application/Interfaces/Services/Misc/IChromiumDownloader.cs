namespace Beatport2Rss.Application.Interfaces.Services.Misc;

public interface IChromiumDownloader
{
    Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default);
}
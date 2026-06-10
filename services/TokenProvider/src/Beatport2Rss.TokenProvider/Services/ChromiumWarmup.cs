using Beatport2Rss.TokenProvider.Services.Interfaces;

namespace Beatport2Rss.TokenProvider.Services;

internal sealed class ChromiumWarmup(IChromiumDownloader chromiumDownloader) :
    IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) =>
        chromiumDownloader.EnsureInstalledAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
using Beatport2Rss.TokenInterceptor.Interfaces;

namespace Beatport2Rss.TokenInterceptor.HostedServices;

internal sealed class ChromiumWarmup(IChromiumDownloader chromiumDownloader) :
    IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) =>
        chromiumDownloader.EnsureInstalledAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
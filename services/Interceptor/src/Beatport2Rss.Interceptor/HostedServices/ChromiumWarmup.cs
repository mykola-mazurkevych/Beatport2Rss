using Beatport2Rss.Interceptor.Interfaces;

namespace Beatport2Rss.Interceptor.HostedServices;

internal sealed class ChromiumWarmup(IChromiumDownloader chromiumDownloader) :
    IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) =>
        chromiumDownloader.EnsureInstalledAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
using Beatport2Rss.Common.Messaging.Interfaces;

using Microsoft.Extensions.Hosting;

namespace Beatport2Rss.Api.Infrastructure.BackgroundServices;

internal sealed class OutboxBackgroundService(
    IOutboxDispatcher outboxDispatcher) :
    BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        outboxDispatcher.DispatchAsync(stoppingToken);
}
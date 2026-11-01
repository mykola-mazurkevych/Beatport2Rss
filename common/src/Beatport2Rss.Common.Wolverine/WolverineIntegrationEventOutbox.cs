using Beatport2Rss.Common.IntegrationEvents;

using Microsoft.EntityFrameworkCore;

using Wolverine.EntityFrameworkCore;

namespace Beatport2Rss.Common.Wolverine;

internal sealed class WolverineIntegrationEventOutbox<TDbContext>(IDbContextOutbox<TDbContext> outbox) :
    IIntegrationEventOutbox
    where TDbContext : DbContext
{
    public async Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent =>
        await outbox.PublishAsync(integrationEvent);
}
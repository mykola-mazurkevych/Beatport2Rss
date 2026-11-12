using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;

namespace Beatport2Rss.Collector.Application.UseCases.Releases.Commands;

public sealed record QueueReleaseCollectionCommand;

public sealed class QueueReleaseCollectionCommandHandler(ISubscriptionCommandRepository subscriptions)
{
    public async Task<IEnumerable<object>> HandleAsync(QueueReleaseCollectionCommand command, CancellationToken cancellationToken)
    {
        var subscriptionIds = await subscriptions.FindAllToRefreshAsync(cancellationToken);
        return subscriptionIds.Select(id => new CollectReleasesCommand(id.Value));
    }
}
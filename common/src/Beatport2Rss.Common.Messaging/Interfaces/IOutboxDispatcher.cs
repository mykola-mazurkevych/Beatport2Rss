namespace Beatport2Rss.Common.Messaging.Interfaces;

public interface IOutboxDispatcher
{
    Task DispatchAsync(CancellationToken cancellationToken = default);
}
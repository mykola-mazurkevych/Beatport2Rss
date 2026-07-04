namespace Beatport2Rss.Common.Messaging.Interfaces;

public interface IPublisher
{
    Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default);
}
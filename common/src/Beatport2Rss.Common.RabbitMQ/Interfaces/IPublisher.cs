namespace Beatport2Rss.Common.RabbitMQ.Interfaces;

public interface IPublisher
{
    Task PublishAsync(
        object message,
        CancellationToken cancellationToken = default);
}
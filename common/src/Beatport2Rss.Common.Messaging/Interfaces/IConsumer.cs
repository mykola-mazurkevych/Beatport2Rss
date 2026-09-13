namespace Beatport2Rss.Common.Messaging.Interfaces;

public interface IConsumer<out TMessage>
    where TMessage : class
{
    Task ConsumeAsync(
        Func<TMessage, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}
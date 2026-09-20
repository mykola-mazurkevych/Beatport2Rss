namespace Beatport2Rss.Common.IntegrationEvents;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredAt { get; }
}
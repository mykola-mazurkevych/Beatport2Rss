namespace Beatport2Rss.Common.IntegrationEvents;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}
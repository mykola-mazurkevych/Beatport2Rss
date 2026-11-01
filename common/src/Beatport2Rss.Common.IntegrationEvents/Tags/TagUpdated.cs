namespace Beatport2Rss.Common.IntegrationEvents.Tags;

[IntegrationEventTopic("tag.updated")]
public sealed record TagUpdated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId,
    string Name) :
    IIntegrationEvent;
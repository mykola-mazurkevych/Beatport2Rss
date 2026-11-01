namespace Beatport2Rss.Common.IntegrationEvents.Tags;

[IntegrationEventTopic("tag.deleted")]
public sealed record TagDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId) :
    IIntegrationEvent;
namespace Beatport2Rss.Common.IntegrationEvents.Tags;

public sealed record TagDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId) :
    IIntegrationEvent;
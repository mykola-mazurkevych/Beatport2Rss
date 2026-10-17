namespace Beatport2Rss.Common.IntegrationEvents.Tags;

public sealed record TagUpdated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId,
    string Name) :
    IIntegrationEvent;
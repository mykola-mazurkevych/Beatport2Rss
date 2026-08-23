namespace Beatport2Rss.Common.IntegrationEvents.V1.Tags;

public sealed record TagUpdatedV1(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid TagId,
    string Name) :
    IIntegrationEvent;
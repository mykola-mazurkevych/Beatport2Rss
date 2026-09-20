namespace Beatport2Rss.Common.IntegrationEvents.V1.Tags;

public sealed record TagUpdatedV1(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId,
    string Name) :
    IIntegrationEvent;
namespace Beatport2Rss.Common.IntegrationEvents.V1.Tags;

public sealed record TagDeletedV1(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId) :
    IIntegrationEvent;
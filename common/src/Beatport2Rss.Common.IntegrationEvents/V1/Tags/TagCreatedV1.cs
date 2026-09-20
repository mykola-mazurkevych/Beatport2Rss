namespace Beatport2Rss.Common.IntegrationEvents.V1.Tags;

public sealed record TagCreatedV1(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId,
    Guid UserId,
    string Name) :
    IIntegrationEvent;
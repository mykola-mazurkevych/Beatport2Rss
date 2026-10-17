namespace Beatport2Rss.Common.IntegrationEvents.Tags;

public sealed record TagCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid TagId,
    Guid UserId,
    string Name) :
    IIntegrationEvent;
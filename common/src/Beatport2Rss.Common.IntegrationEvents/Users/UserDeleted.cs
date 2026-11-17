namespace Beatport2Rss.Common.IntegrationEvents.Users;

[IntegrationEventTopic("user.deleted")]
public sealed record UserDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid UserId) :
    IIntegrationEvent;
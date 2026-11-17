namespace Beatport2Rss.Common.IntegrationEvents.Users;

[IntegrationEventTopic("user.created")]
public sealed record UserCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid UserId) :
    IIntegrationEvent;
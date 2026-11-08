using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Common.IntegrationEvents.Subscriptions;

[IntegrationEventTopic("subscription.requested")]
public sealed record SubscriptionRequested(
    Guid Id,
    DateTimeOffset OccurredAt,
    SubscriptionType Type,
    int BeatportId) :
    IIntegrationEvent;
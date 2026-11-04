using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Common.IntegrationEvents.Subscriptions;

[IntegrationEventTopic("subscription.created")]
public sealed record SubscriptionCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid SubscriptionId,
    string Name,
    SubscriptionType Type,
    int BeatportId,
    string BeatportSlug) :
    IIntegrationEvent;
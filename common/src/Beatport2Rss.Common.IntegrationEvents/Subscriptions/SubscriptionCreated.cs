using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Common.IntegrationEvents.Subscriptions;

[IntegrationEventTopic("subscription.created")]
public sealed record SubscriptionCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid SubscriptionId,
    SubscriptionType Type,
    string Name,
    int BeatportId,
    string BeatportSlug,
    Uri ImageUri,
    Uri? Website,
    string? Bio) :
    IIntegrationEvent;
namespace Beatport2Rss.Common.IntegrationEvents.V1.Subscriptions;

public sealed record SubscriptionCreatedV1(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid SubscriptionId,
    int Type,
    int BeatportId,
    string BeatportSlug) :
    IIntegrationEvent;
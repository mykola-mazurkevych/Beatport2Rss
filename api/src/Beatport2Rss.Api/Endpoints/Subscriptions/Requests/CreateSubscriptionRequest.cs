using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Subscriptions;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Requests;

internal sealed record CreateSubscriptionRequest(
    SubscriptionType Type,
    BeatportId BeatportId,
    string? CountryCode);
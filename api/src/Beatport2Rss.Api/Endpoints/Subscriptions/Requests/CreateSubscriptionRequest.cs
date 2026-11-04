using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Requests;

internal sealed record CreateSubscriptionRequest(
    SubscriptionType Type,
    BeatportId BeatportId,
    string? CountryCode);
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Requests;

internal sealed record CreateSubscriptionRequest(
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId);
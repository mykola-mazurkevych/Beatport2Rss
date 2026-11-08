using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Requests;

internal sealed record RequestSubscriptionRequest(
    SubscriptionType Type,
    BeatportId BeatportId);
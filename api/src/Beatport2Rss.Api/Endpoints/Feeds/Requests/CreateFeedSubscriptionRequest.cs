using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Endpoints.Feeds.Requests;

internal sealed record CreateFeedSubscriptionRequest(
    Slug SubscriptionSlug);
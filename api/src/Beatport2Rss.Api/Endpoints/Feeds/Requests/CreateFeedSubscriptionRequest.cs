using Beatport2Rss.Common.Domain.ValueObjects;

namespace Beatport2Rss.Api.Endpoints.Feeds.Requests;

internal sealed record CreateFeedSubscriptionRequest(
    Slug SubscriptionSlug);
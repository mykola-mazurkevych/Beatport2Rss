using Beatport2Rss.Api.Domain.Common.ValueObjects;

namespace Beatport2Rss.Api.Endpoints.Feeds.Requests;

internal sealed record CreateFeedSubscriptionRequest(
    Slug SubscriptionSlug);
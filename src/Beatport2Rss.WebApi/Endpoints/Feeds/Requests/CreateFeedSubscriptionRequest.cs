using Beatport2Rss.Domain.Common.ValueObjects;
namespace Beatport2Rss.WebApi.Endpoints.Feeds.Requests;

internal sealed record CreateFeedSubscriptionRequest(
    Slug SubscriptionSlug);
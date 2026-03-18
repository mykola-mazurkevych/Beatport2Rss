using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Requests;

internal sealed record CreateSubscriptionTagRequest(
    Slug TagSlug);
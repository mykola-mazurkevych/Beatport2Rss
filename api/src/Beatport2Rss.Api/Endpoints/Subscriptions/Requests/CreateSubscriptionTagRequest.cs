using Beatport2Rss.Api.Domain.Common.ValueObjects;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Requests;

internal sealed record CreateSubscriptionTagRequest(
    Slug TagSlug);
using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Requests;

internal sealed record CreateSubscriptionTagRequest(
    Slug TagSlug);
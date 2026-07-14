using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Requests;

internal sealed record CreateSubscriptionTagRequest(
    Slug TagSlug);
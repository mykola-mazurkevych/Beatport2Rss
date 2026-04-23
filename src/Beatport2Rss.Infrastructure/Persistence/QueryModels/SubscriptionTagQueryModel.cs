using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionTagQueryModel(
    SubscriptionId SubscriptionId,
    TagId TagId,
    UserId UserId,
    TagName Name,
    Slug Slug);
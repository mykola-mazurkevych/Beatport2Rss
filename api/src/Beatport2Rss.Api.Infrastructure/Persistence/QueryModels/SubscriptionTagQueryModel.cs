using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionTagQueryModel(
    SubscriptionId SubscriptionId,
    TagId TagId,
    UserId UserId,
    TagName Name,
    Slug Slug);
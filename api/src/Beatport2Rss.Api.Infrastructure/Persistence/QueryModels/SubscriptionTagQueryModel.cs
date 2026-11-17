using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.Domain.Users;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionTagQueryModel(
    SubscriptionId SubscriptionId,
    TagId TagId,
    UserId UserId,
    TagName Name,
    Slug Slug);
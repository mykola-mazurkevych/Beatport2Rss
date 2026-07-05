using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionTagQueryModel(
    SubscriptionId SubscriptionId,
    TagId TagId,
    UserId UserId,
    TagName Name,
    Slug Slug);
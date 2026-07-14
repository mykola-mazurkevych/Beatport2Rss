using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionTagQueryModel(
    SubscriptionId SubscriptionId,
    TagId TagId,
    UserId UserId,
    TagName Name,
    Slug Slug);
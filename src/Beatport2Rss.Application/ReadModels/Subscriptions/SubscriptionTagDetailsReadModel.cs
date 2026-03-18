using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Application.ReadModels.Subscriptions;

public sealed record SubscriptionTagDetailsReadModel(
    TagName Name,
    Slug Slug);
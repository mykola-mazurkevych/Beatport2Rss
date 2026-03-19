using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Application.Dtos.Subscriptions;

public sealed record SubscriptionTagDto(
    TagName Name,
    Slug Slug);
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.Domain.ValueObjects;

namespace Beatport2Rss.Api.Application.Dtos.Subscriptions;

public sealed record SubscriptionTagDto(
    TagName Name,
    Slug Slug);
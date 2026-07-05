using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Tags;

namespace Beatport2Rss.Api.Application.Dtos.Subscriptions;

public sealed record SubscriptionTagDto(
    TagName Name,
    Slug Slug);
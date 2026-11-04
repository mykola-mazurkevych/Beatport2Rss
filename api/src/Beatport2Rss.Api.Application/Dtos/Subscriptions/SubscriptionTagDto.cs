using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Api.Application.Dtos.Subscriptions;

public sealed record SubscriptionTagDto(
    TagName Name,
    Slug Slug);
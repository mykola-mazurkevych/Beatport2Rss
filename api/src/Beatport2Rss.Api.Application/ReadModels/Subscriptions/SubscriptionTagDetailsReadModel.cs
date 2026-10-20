using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.Domain.ValueObjects;

namespace Beatport2Rss.Api.Application.ReadModels.Subscriptions;

public sealed record SubscriptionTagDetailsReadModel
{
    public required TagName Name { get; init; }
    public required Slug Slug { get; init; }
}
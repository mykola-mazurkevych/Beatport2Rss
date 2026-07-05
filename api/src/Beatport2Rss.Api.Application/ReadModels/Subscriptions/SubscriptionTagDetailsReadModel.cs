using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Tags;

namespace Beatport2Rss.Api.Application.ReadModels.Subscriptions;

public sealed record SubscriptionTagDetailsReadModel
{
    public required TagName Name { get; init; }
    public required Slug Slug { get; init; }
}
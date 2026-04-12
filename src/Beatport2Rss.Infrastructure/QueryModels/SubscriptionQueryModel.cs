using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Infrastructure.QueryModels;

internal sealed record SubscriptionQueryModel(
    SubscriptionId Id,
    DateTimeOffset CreatedAt,
    SubscriptionName Name,
    Slug Slug,
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    Uri ImageUri,
    DateTimeOffset? RefreshedAt,
    IReadOnlyCollection<SubscriptionTagQueryModel> Tags) :
    IQueryModel<SubscriptionId>;
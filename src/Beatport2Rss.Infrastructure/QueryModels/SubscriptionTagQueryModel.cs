namespace Beatport2Rss.Infrastructure.QueryModels;

internal sealed record SubscriptionTagQueryModel(
    Guid UserId,
    string Name,
    string Slug);
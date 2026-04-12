namespace Beatport2Rss.Infrastructure.Persistence.QueryModels;

internal sealed record SubscriptionTagQueryModel(
    Guid UserId,
    string Name,
    string Slug);
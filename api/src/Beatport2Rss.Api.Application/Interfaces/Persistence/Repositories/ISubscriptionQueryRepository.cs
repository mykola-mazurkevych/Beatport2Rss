using Beatport2Rss.Api.Application.ReadModels.Subscriptions;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionQueryRepository
{
    IQueryable<SubscriptionPaginableReadModel> GetSubscriptionPaginableReadModelsAsQueryable(UserId userId);

    Task<bool> ExistsAsync(
        Slug slug,
        CancellationToken cancellationToken = default);

    Task<SubscriptionId> LoadSubscriptionIdAsync(
        Slug slug,
        CancellationToken cancellationToken = default);

    Task<SubscriptionDetailsReadModel> LoadWithUserTagsAsync(
        Slug slug,
        UserId userId,
        CancellationToken cancellationToken = default);
}
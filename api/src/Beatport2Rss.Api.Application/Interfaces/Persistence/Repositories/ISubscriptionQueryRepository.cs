using Beatport2Rss.Api.Application.ReadModels.Subscriptions;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

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
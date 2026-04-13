using Beatport2Rss.Application.Interfaces.Models.Subscriptions;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionQueryRepository
{
    Task<bool> ExistsAsync(Slug slug, CancellationToken cancellationToken = default);

    Task<SubscriptionId> LoadSubscriptionIdAsync(Slug slug, CancellationToken cancellationToken = default);
    Task<IHaveSubscriptionDetails> LoadWithUserTagsAsync(Slug slug, UserId userId, CancellationToken cancellationToken = default);
}
using Beatport2Rss.Application.ReadModels.Subscriptions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionQueryRepository(
    IQueryable<SubscriptionQueryModel> subscriptionQueryModels) :
    QueryRepository<SubscriptionQueryModel, SubscriptionId>(subscriptionQueryModels),
    ISubscriptionQueryRepository
{
    private readonly IQueryable<SubscriptionQueryModel> _subscriptionQueryModels = subscriptionQueryModels;

    public IQueryable<SubscriptionPaginableReadModel> GetSubscriptionPaginableReadModelsAsQueryable(UserId userId) =>
        from subscriptionQueryModel in _subscriptionQueryModels
        select new SubscriptionPaginableReadModel
        {
            Id = subscriptionQueryModel.Id,
            CreatedAt = subscriptionQueryModel.CreatedAt,
            Name = subscriptionQueryModel.Name,
            Slug = subscriptionQueryModel.Slug,
            BeatportType = subscriptionQueryModel.BeatportType,
            BeatportId = subscriptionQueryModel.BeatportId,
            BeatportSlug = subscriptionQueryModel.BeatportSlug,
            ImageUri = subscriptionQueryModel.ImageUri,
            RefreshedAt = subscriptionQueryModel.RefreshedAt,
            Tags = subscriptionQueryModel.Tags
                .Where(subscriptionTagQueryModel => subscriptionTagQueryModel.UserId == userId)
                .Select(subscriptionTagQueryModel =>new SubscriptionTagDetailsReadModel
                    {
                        Name = subscriptionTagQueryModel.Name,
                        Slug = subscriptionTagQueryModel.Slug,
                    }),
        };

    public Task<bool> ExistsAsync(
        Slug slug,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            subscriptionQueryModel => subscriptionQueryModel.Slug == slug,
            cancellationToken);

    public Task<SubscriptionId> LoadSubscriptionIdAsync(
        Slug slug,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            subscriptionQueryModel => subscriptionQueryModel.Slug == slug,
            subscriptionQueryModel => subscriptionQueryModel.Id,
            cancellationToken);

    public Task<SubscriptionDetailsReadModel> LoadWithUserTagsAsync(
        Slug slug,
        UserId userId,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            subscriptionQueryModel => subscriptionQueryModel.Slug == slug,
            subscriptionQueryModel => new SubscriptionDetailsReadModel(
                subscriptionQueryModel.Id,
                subscriptionQueryModel.Name,
                subscriptionQueryModel.Slug,
                subscriptionQueryModel.BeatportType,
                subscriptionQueryModel.BeatportId,
                subscriptionQueryModel.BeatportSlug,
                subscriptionQueryModel.ImageUri,
                subscriptionQueryModel.Tags
                    .Where(subscriptionTagQueryModel => subscriptionTagQueryModel.UserId == userId)
                    .Select(subscriptionTagQueryModel => new SubscriptionTagDetailsReadModel
                        {
                            Name = subscriptionTagQueryModel.Name,
                            Slug = subscriptionTagQueryModel.Slug,
                        }
                    ),
                subscriptionQueryModel.CreatedAt,
                subscriptionQueryModel.RefreshedAt),
            cancellationToken);
}
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.ReadModels.Subscriptions;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

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
            Country = subscriptionQueryModel.Country,
            SubscribersCount = subscriptionQueryModel.SubscribersCount,
            Tags = subscriptionQueryModel.Tags
                .Where(subscriptionTagQueryModel => subscriptionTagQueryModel.UserId == userId)
                .Select(subscriptionTagQueryModel => new SubscriptionTagDetailsReadModel
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
                subscriptionQueryModel.Country,
                subscriptionQueryModel.Tags
                    .Where(subscriptionTagQueryModel => subscriptionTagQueryModel.UserId == userId)
                    .Select(subscriptionTagQueryModel => new SubscriptionTagDetailsReadModel
                    {
                        Name = subscriptionTagQueryModel.Name,
                        Slug = subscriptionTagQueryModel.Slug,
                    })),
            cancellationToken);
}
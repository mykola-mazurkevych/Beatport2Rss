using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Models.Subscriptions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.QueryModels;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionQueryRepository(
    IQueryable<SubscriptionQueryModel> subscriptionQueryModels) :
    QueryRepository<SubscriptionQueryModel, SubscriptionId>(subscriptionQueryModels),
    ISubscriptionQueryRepository
{
    public Task<bool> ExistsAsync(Slug slug, CancellationToken cancellationToken = default) =>
        base.ExistsAsync(
            subscriptionQueryModel => subscriptionQueryModel.Slug == slug,
            cancellationToken);

    public Task<SubscriptionId> LoadSubscriptionIdAsync(Slug slug, CancellationToken cancellationToken = default) =>
        LoadAsync(
            subscriptionQueryModel => subscriptionQueryModel.Slug == slug,
            subscriptionQueryModel => subscriptionQueryModel.Id,
            cancellationToken);

    public async Task<IHaveSubscriptionDetails> LoadWithUserTagsAsync(
        Slug slug,
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        var subscription = await LoadAsync(
            subscriptionQueryModel => subscriptionQueryModel.Slug == slug,
            SubscriptionDetailsProjection.Selector,
            cancellationToken);

        var tags = subscription.Tags
            .Where(tagPayload => tagPayload.UserId == userId.Value)
            .Select(tagPayload =>
                new SubscriptionTagDetails(
                    TagName.Create(tagPayload.Name),
                    Slug.Create(tagPayload.Slug)))
            .Cast<IHaveSubscriptionTagDetails>()
            .ToArray();

        return new SubscriptionDetails(
            subscription.Id,
            subscription.Name,
            subscription.Slug,
            subscription.BeatportType,
            subscription.BeatportId,
            subscription.BeatportSlug,
            subscription.ImageUri,
            tags,
            subscription.CreatedAt,
            subscription.RefreshedAt);
    }

    private sealed record SubscriptionDetailsProjection(
        SubscriptionId Id,
        SubscriptionName Name,
        Slug Slug,
        BeatportSubscriptionType BeatportType,
        BeatportId BeatportId,
        BeatportSlug BeatportSlug,
        Uri ImageUri,
        IReadOnlyCollection<SubscriptionTagQueryModel> Tags,
        DateTimeOffset? CreatedAt,
        DateTimeOffset? RefreshedAt)
    {
        public static Expression<Func<SubscriptionQueryModel, SubscriptionDetailsProjection>> Selector =>
            subscriptionQueryModel => new SubscriptionDetailsProjection(
                subscriptionQueryModel.Id,
                subscriptionQueryModel.Name,
                subscriptionQueryModel.Slug,
                subscriptionQueryModel.BeatportType,
                subscriptionQueryModel.BeatportId,
                subscriptionQueryModel.BeatportSlug,
                subscriptionQueryModel.ImageUri,
                subscriptionQueryModel.Tags,
                subscriptionQueryModel.CreatedAt,
                subscriptionQueryModel.RefreshedAt);
    }

    private sealed record SubscriptionDetails(
        SubscriptionId Id,
        SubscriptionName Name,
        Slug Slug,
        BeatportSubscriptionType BeatportType,
        BeatportId BeatportId,
        BeatportSlug BeatportSlug,
        Uri ImageUri,
        IEnumerable<IHaveSubscriptionTagDetails> Tags,
        DateTimeOffset? CreatedAt,
        DateTimeOffset? RefreshedAt) :
        IHaveSubscriptionDetails;

    private sealed record SubscriptionTagDetails(
        TagName Name,
        Slug Slug) :
        IHaveSubscriptionTagDetails;
}
using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.ReadModels.Releases;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Builder.Infrastructure.Persistence.QueryModels;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class ReleaseQueryRepository(
    DbSet<Feed> feeds,
    DbSet<Release> releases,
    DbSet<Subscription> subscriptions,
    DbSet<Tag> tags) :
    IReleaseQueryRepository
{
    public async Task<IReadOnlyCollection<ReleaseReadModel>> GetForFeedAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default)
    {
        var feed = await feeds
            .AsNoTracking()
            .Include(value => value.Subscriptions)
            .SingleOrDefaultAsync(value => value.Id == feedId, cancellationToken)
            ?? throw new InvalidOperationException($"Feed '{feedId}' was not found.");

        var feedSubscriptionIds = feed.Subscriptions
            .Select(feedSubscription => feedSubscription.SubscriptionId)
            .ToHashSet();

        if (feedSubscriptionIds.Count == 0)
        {
            return [];
        }

        var matchingReleases = await releases
            .AsNoTracking()
            .Include(release => release.Artists)
            .Include(release => release.Tracks)
                .ThenInclude(track => track.Artists)
            .Where(release =>
                feedSubscriptionIds.Contains(release.LabelId) ||
                release.Artists.Any(artist => feedSubscriptionIds.Contains(artist.ArtistId)))
            .ToArrayAsync(cancellationToken);

        var subscriptionIds = matchingReleases
            .SelectMany(release => release.Artists.Select(artist => artist.ArtistId)
                .Append(release.LabelId)
                .Concat(release.Tracks.SelectMany(track => track.Artists.Select(artist => artist.ArtistId))))
            .Distinct()
            .ToArray();
        var matchingSubscriptions = await subscriptions
            .AsNoTracking()
            .Include(subscription => subscription.Tags)
            .Where(subscription => subscriptionIds.Contains(subscription.Id))
            .ToDictionaryAsync(subscription => subscription.Id, cancellationToken);

        var tagIds = matchingSubscriptions.Values
            .SelectMany(subscription => subscription.Tags.Select(tag => tag.TagId))
            .Distinct()
            .ToArray();
        var tagNames = await tags
            .AsNoTracking()
            .Where(tag => tagIds.Contains(tag.Id))
            .ToDictionaryAsync(tag => tag.Id, tag => tag.Name.Value, cancellationToken);

        return matchingReleases
            .Select(release => ToQueryModel(release, feedSubscriptionIds, matchingSubscriptions, tagNames))
            .Select(ToReadModel)
            .ToArray();
    }

    private static ReleaseQueryModel ToQueryModel(
        Release release,
        IReadOnlySet<Beatport2Rss.Common.Domain.Subscriptions.SubscriptionId> feedSubscriptionIds,
        IReadOnlyDictionary<Beatport2Rss.Common.Domain.Subscriptions.SubscriptionId, Subscription> subscriptions,
        IReadOnlyDictionary<Beatport2Rss.Common.Domain.Tags.TagId, string> tagNames)
    {
        ReleaseContributorQueryModel GetContributor(
            Beatport2Rss.Common.Domain.Subscriptions.SubscriptionId subscriptionId)
        {
            var subscription = subscriptions[subscriptionId];
            var subscriptionTags = subscription.Tags
                .Select(subscriptionTag => tagNames[subscriptionTag.TagId])
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new ReleaseContributorQueryModel(
                subscriptionId.Value,
                subscription.Name.Value,
                subscriptionTags);
        }

        var releaseSubscriptionIds = release.Artists
            .Select(artist => artist.ArtistId)
            .Append(release.LabelId)
            .Distinct()
            .ToArray();

        return new ReleaseQueryModel(
            release.Id.Value,
            release.BeatportId.Value,
            release.BeatportSlug.Value,
            release.CreatedAt,
            release.Name.Value,
            release.ImageUri,
            release.CatalogNumber.Value,
            release.ReleaseDate,
            GetContributor(release.LabelId),
            release.Artists.Select(artist => GetContributor(artist.ArtistId)).ToArray(),
            release.Tracks
                .OrderBy(track => track.Number)
                .Select(track => new ReleaseTrackQueryModel(
                    track.Number,
                    track.Name.Value,
                    track.MixName.Value,
                    track.Length,
                    track.Artists.Select(artist => GetContributor(artist.ArtistId)).ToArray()))
                .ToArray(),
            releaseSubscriptionIds
                .Where(feedSubscriptionIds.Contains)
                .Select(subscriptionId => subscriptions[subscriptionId].Name.Value)
                .ToArray());
    }

    private static ReleaseReadModel ToReadModel(ReleaseQueryModel queryModel) =>
        new(
            ReleaseId.Create(queryModel.Id),
            BeatportId.Create(queryModel.BeatportId),
            BeatportSlug.Create(queryModel.BeatportSlug),
            queryModel.CreatedAt,
            ReleaseName.Create(queryModel.Name),
            queryModel.ImageUri,
            CatalogNumber.Create(queryModel.CatalogNumber),
            queryModel.ReleaseDate,
            ToReadModel(queryModel.Label),
            queryModel.Artists.Select(ToReadModel).ToArray(),
            queryModel.Tracks.Select(track => new TrackReadModel(
                track.Number,
                TrackName.Create(track.Name),
                MixName.Create(track.MixName),
                track.Length,
                track.Artists.Select(ToReadModel).ToArray())).ToArray(),
            queryModel.Categories);

    private static ContributorReadModel ToReadModel(ReleaseContributorQueryModel queryModel) =>
        new(
            SubscriptionId.Create(queryModel.SubscriptionId),
            SubscriptionName.Create(queryModel.Name),
            queryModel.Tags.Select(TagName.Create).ToArray());
}

using Beatport2Rss.Builder.Application.Dtos;
using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Common;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Releases.Commands;

public sealed record CreateReleaseCommand(
    ReleaseId ReleaseId,
    DateTimeOffset CreatedAt,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    IReadOnlyCollection<BeatportId> ArtistBeatportIds,
    IReadOnlyCollection<BeatportId> RemixerBeatportIds,
    BeatportId LabelBeatportId,
    ReleaseName ReleaseName,
    DateOnly ReleaseDate,
    CatalogNumber CatalogNumber,
    Uri ImageUri,
    int TracksCount,
    IReadOnlyCollection<TrackDto> Tracks);

public static class CreateReleaseCommandHandler
{
    public static async Task HandleAsync(
        CreateReleaseCommand createReleaseCommand,
        IClock clock,
        IBeatportUriBuilder beatportUriBuilder,
        IReleaseCommandRepository releaseCommandRepository,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var exists = await releaseCommandRepository.ExistsAsync(createReleaseCommand.ReleaseId, cancellationToken);
        if (exists)
        {
            // log info
            return;
        }

        HashSet<BeatportId> subscriptionBeatportIds = [createReleaseCommand.LabelBeatportId];
        subscriptionBeatportIds.UnionWith(createReleaseCommand.ArtistBeatportIds);
        subscriptionBeatportIds.UnionWith(createReleaseCommand.RemixerBeatportIds);
        // Technically, artists/remixers from tracks should alredy be included in release's artists/tracks
        subscriptionBeatportIds.UnionWith(createReleaseCommand.Tracks.SelectMany(track => track.ArtistBeatportIds));
        subscriptionBeatportIds.UnionWith(createReleaseCommand.Tracks.SelectMany(track => track.RemixerBeatportIds));

        var subscriptions = await subscriptionCommandRepository.FindAllAsync(subscriptionBeatportIds, cancellationToken);
        var subscriptionsMap = subscriptions.ToDictionary(subscription => (subscription.Type, subscription.BeatportId));

        var label = subscriptionsMap[(SubscriptionType.Label, createReleaseCommand.LabelBeatportId)];

        var releaseArtists = GetArtists(
            createReleaseCommand.ReleaseId,
            createReleaseCommand.ArtistBeatportIds,
            createReleaseCommand.RemixerBeatportIds,
            subscriptionsMap,
            ReleaseArtist.Create);

        var tracks = GetTracks(
            createReleaseCommand.ReleaseId,
            createReleaseCommand.Tracks,
            subscriptionsMap,
            clock);

        var release = Release.Create(
            createReleaseCommand.ReleaseId,
            createReleaseCommand.CreatedAt,
            createReleaseCommand.BeatportId,
            createReleaseCommand.BeatportSlug,
            beatportUri: beatportUriBuilder.Build(
                createReleaseCommand.BeatportId.Value,
                createReleaseCommand.BeatportSlug.Value),
            createReleaseCommand.ReleaseName,
            createReleaseCommand.ReleaseDate,
            label.Id,
            createReleaseCommand.CatalogNumber,
            createReleaseCommand.ImageUri,
            createReleaseCommand.TracksCount,
            releaseArtists,
            tracks);
        await releaseCommandRepository.AddAsync(release, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static HashSet<Track> GetTracks(
        ReleaseId releaseId,
        IEnumerable<TrackDto> trackDtos,
        Dictionary<(SubscriptionType Type, BeatportId BeatportId), Subscription> subscriptionsMap,
        IClock clock)
    {
        HashSet<Track> tracks = [];

        foreach (var trackDto in trackDtos.OrderBy(track => track.Number))
        {
            var trackId = TrackId.Create(Guid.CreateVersion7());

            var trackArtsits = GetArtists(
                trackId,
                trackDto.ArtistBeatportIds,
                trackDto.RemixerBeatportIds,
                subscriptionsMap,
                TrackArtist.Create);

            var track = Track.Create(
                trackId,
                createdAt: clock.UtcNow,
                releaseId,
                trackDto.Number,
                trackDto.Name,
                trackDto.MixName,
                trackDto.Length,
                trackDto.SampleUri,
                trackArtsits);

            tracks.Add(track);
        }

        return tracks;
    }

    private static HashSet<TArtist> GetArtists<TOwnerId, TArtist>(
        TOwnerId ownerId,
        IEnumerable<BeatportId> artistBeatportIds,
        IEnumerable<BeatportId> remixerBeatportIds,
        Dictionary<(SubscriptionType Type, BeatportId BeatportId), Subscription> subscriptionsMap,
        Func<TOwnerId, SubscriptionId, ArtistType, TArtist> create) =>
    [
        .. artistBeatportIds
            .Select(beatportId => create(ownerId, subscriptionsMap[(SubscriptionType.Artist, beatportId)].Id, ArtistType.Artist))
            .Union(remixerBeatportIds.Select(beatportId => create(ownerId, subscriptionsMap[(SubscriptionType.Artist, beatportId)].Id, ArtistType.Remixer)))
    ];
}
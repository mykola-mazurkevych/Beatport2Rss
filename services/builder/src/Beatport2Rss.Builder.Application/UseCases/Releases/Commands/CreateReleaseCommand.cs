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
    IReadOnlyCollection<ArtistDto> Artists,
    IReadOnlyCollection<ArtistDto> Remixers,
    LabelDto Label,
    ReleaseName ReleaseName,
    DateOnly ReleaseDate,
    CatalogNumber CatalogNumber,
    Uri ImageUri,
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

        HashSet<BeatportId> subscriptionBeatportIds = [createReleaseCommand.Label.BeatportId];
        subscriptionBeatportIds.UnionWith(createReleaseCommand.Artists.Select(artist => artist.BeatportId));
        subscriptionBeatportIds.UnionWith(createReleaseCommand.Remixers.Select(remixer => remixer.BeatportId));
        // Technically, artists/remixers from tracks should alredy be included in release's artists/tracks
        subscriptionBeatportIds.UnionWith(createReleaseCommand.Tracks.SelectMany(track => track.Artists).Select(artist => artist.BeatportId));
        subscriptionBeatportIds.UnionWith(createReleaseCommand.Tracks.SelectMany(track => track.Remixers).Select(remixer => remixer.BeatportId));

        var subscriptions = await subscriptionCommandRepository.FindAllAsync(subscriptionBeatportIds, cancellationToken);
        var subscriptionsMap = subscriptions
            .ToDictionary(subscription => (subscription.Type, subscription.BeatportId));

        var label = await GetSubscriptionAsync(
            subscriptionsMap,
            SubscriptionType.Label,
            createReleaseCommand.Label.BeatportId,
            createReleaseCommand.Label.Name,
            clock,
            subscriptionCommandRepository,
            cancellationToken);

        var releaseArtists = await GetArtistsAsync(
            createReleaseCommand.ReleaseId,
            createReleaseCommand.Artists,
            createReleaseCommand.Remixers,
            subscriptionsMap,
            clock,
            subscriptionCommandRepository,
            ReleaseArtist.Create,
            cancellationToken);

        var tracks = await GetTracksAsync(
            createReleaseCommand.ReleaseId,
            createReleaseCommand.Tracks,
            subscriptionsMap,
            clock,
            subscriptionCommandRepository,
            cancellationToken);

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
            releaseArtists,
            tracks);
        await releaseCommandRepository.AddAsync(release, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static async Task<IEnumerable<Track>> GetTracksAsync(
        ReleaseId releaseId,
        IEnumerable<TrackDto> trackDtos,
        Dictionary<(SubscriptionType Type, BeatportId BeatportId), Subscription> subscriptionsMap,
        IClock clock,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        CancellationToken cancellationToken)
    {
        HashSet<Track> tracks = [];

        foreach (var trackDto in trackDtos.OrderBy(t => t.Number))
        {
            var trackId = TrackId.Create(Guid.CreateVersion7());

            var trackArtsits = await GetArtistsAsync(
                trackId,
                trackDto.Artists,
                trackDto.Remixers,
                subscriptionsMap,
                clock,
                subscriptionCommandRepository,
                TrackArtist.Create,
                cancellationToken);

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

    private static async Task<IEnumerable<TArtist>> GetArtistsAsync<TOwnerId, TArtist>(
        TOwnerId ownerId,
        IEnumerable<ArtistDto> artistDtos,
        IEnumerable<ArtistDto> remixerDtos,
        Dictionary<(SubscriptionType Type, BeatportId BeatportId), Subscription> subscriptionsMap,
        IClock clock,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        Func<TOwnerId, SubscriptionId, ArtistType, TArtist> create,
        CancellationToken cancellationToken)
    {
        HashSet<TArtist> artists = [];

        await ProcessAsync(artistDtos, ArtistType.Artist);
        await ProcessAsync(remixerDtos, ArtistType.Remixer);

        return artists;

        async Task ProcessAsync(IEnumerable<ArtistDto> dtos, ArtistType type)
        {
            foreach (var dto in dtos)
            {
                var subscription = await GetSubscriptionAsync(
                    subscriptionsMap,
                    SubscriptionType.Artist,
                    dto.BeatportId,
                    dto.Name,
                    clock,
                    subscriptionCommandRepository,
                    cancellationToken);

                var artist = create(ownerId, subscription.Id, type);
                artists.Add(artist);
            }
        }
    }

    private static async Task<Subscription> GetSubscriptionAsync(
        Dictionary<(SubscriptionType Type, BeatportId BeatportId), Subscription> subscriptionsMap,
        SubscriptionType type,
        BeatportId beatportId,
        SubscriptionName name,
        IClock clock,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        CancellationToken cancellationToken)
    {
        var key = (type, beatportId);
        if (subscriptionsMap.TryGetValue(key, out var subscription))
        {
            return subscription;
        }

        subscription = await subscriptionCommandRepository.AddAsync(
            Subscription.Create(
                SubscriptionId.Create(Guid.CreateVersion7()),
                createdAt: clock.UtcNow,
                type,
                name,
                beatportId),
            cancellationToken);
        subscriptionsMap[key] = subscription;

        return subscription;
    }
}
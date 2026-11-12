#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable IDE0060 // Remove unused parameter

using System.Globalization;

using Beatport2Rss.Collector.Application.Exceptions;
using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.Interfaces.Services;
using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Releases;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Collector.Application.UseCases.Releases.Commands;

public sealed record CollectReleasesCommand;

internal sealed class CollectReleasesCommandHandler(
    IReleaseCommandRepository releaseCommandRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    ISubscriptionService subscriptionService,
    IBeatportClient beatportClient,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork,
    IClock clock,
    ILogger<CollectReleasesCommand> logger)
{
    private const int InitialPullDaysCount = 10;
    private const int PageSize = 100;

    public async Task HandleAsync(CollectReleasesCommand command, CancellationToken cancellationToken)
    {
        CollectReleasesCommandLoggerMessages.Started(logger);

        var subscription = await subscriptionCommandRepository.FindNextToRefreshAsync(cancellationToken);
        if (subscription is null)
        {
            CollectReleasesCommandLoggerMessages.NoSubscriptions(logger);
            return;
        }

        CollectReleasesCommandLoggerMessages.Processing(logger, subscription.Id);

        subscription.RecordRefreshAttempt(clock.UtcToday);

        var result = await ProcessSubscriptionAsync(subscription, cancellationToken);

        if (result.IsSuccess)
        {
            subscription.MarkRefreshed();
        }
        else
        {
            subscription.RecordRefreshFailure(failReason: result.ErrorMessage);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Result> ProcessSubscriptionAsync(
        Subscription subscriptionToProcess,
        CancellationToken cancellationToken)
    {
        try
        {
            var releaseBeatportIdsResult = await CollectReleaseBeatportIdsAsync(subscriptionToProcess, cancellationToken);
            if (releaseBeatportIdsResult.IsFailed)
            {
                return Result.Fail(releaseBeatportIdsResult);
            }

            var releaseBeatportIds = releaseBeatportIdsResult.Value;
            var existingReleases = await releaseCommandRepository.FindAllAsync(releaseBeatportIds, cancellationToken);
            var releaseBeatportIdsToCollect = releaseBeatportIds.Except(existingReleases.Select(release => release.BeatportId));

            foreach (var releaseBeatportId in releaseBeatportIdsToCollect)
            {

                var beatportReleaseResult = await beatportClient.GetAsync<BeatportReleaseDto>(releaseBeatportId.Value, cancellationToken);
                if (beatportReleaseResult.IsFailed)
                {
                    return Result.Fail(beatportReleaseResult);
                }

                var beatportRelease = beatportReleaseResult.Value!;

                var trackIds = beatportRelease.TrackUris
                    .Select(trackUri => int.Parse(trackUri.Segments.Last().Trim('/'), CultureInfo.InvariantCulture))
                    .ToList();

                var beatportTracksResult = await CollectTracksAsync(trackIds, cancellationToken);
                if (beatportTracksResult.IsFailed)
                {
                    return Result.Fail(beatportTracksResult);
                }

                var beatportTracks = beatportTracksResult.Value ?? [];

                var processSubscriptionsResult = await ProcessSubscriptionsAsync(beatportRelease, beatportTracks, cancellationToken);
                if (processSubscriptionsResult.IsFailed)
                {
                    return Result.Fail(processSubscriptionsResult);
                }

                var releaseId = ReleaseId.Create(Guid.CreateVersion7());
                var release = Release.Create(
                    releaseId,
                    createdAt: clock.UtcNow,
                    releaseBeatportId);
                await releaseCommandRepository.AddAsync(release, cancellationToken);

                var releaseCollected = new ReleaseCollected(
                    Id: Guid.CreateVersion7(),
                    OccurredAt: clock.UtcNow,
                    releaseId.Value,
                    beatportRelease.Id,
                    beatportRelease.Slug,
                    [.. beatportRelease.Artists.Select(artist => artist.Id)],
                    [.. beatportRelease.Remixers.Select(remixer => remixer.Id)],
                    beatportRelease.Label.Id,
                    beatportRelease.CatalogNumber,
                    beatportRelease.Name,
                    beatportRelease.Description,
                    ReleaseDate: beatportRelease.PublishDate, // beatportRelease.NewReleaseDate???
                    beatportRelease.Image.Uri,
                    beatportRelease.TracksCount,
                    [
                        .. beatportTracks.Select(track => new ReleaseCollectedTrack(
                            track.Id,
                            track.Number,
                            track.Name,
                            track.MixName,
                            [.. track.Artists.Select(artist => artist.Id)],
                            [.. track.Remixers.Select(remixer => remixer.Id)],
                            Length: TimeSpan.FromMilliseconds(track.LengthMilliseconds),
                            track.SampleUri))
                    ]);
                await integrationEventOutbox.EnqueueAsync(releaseCollected, cancellationToken);
            }
        }
        catch (RetryableBeatportRequestException)
        {
            throw;
        }
        catch (Exception exception)
        {
            CollectReleasesCommandLoggerMessages.Failed(logger, exception);
            return Result.Fail(exception.Message);
        }

        return Result.Ok();
    }

    private async Task<Result<IReadOnlyCollection<BeatportId>>> CollectReleaseBeatportIdsAsync(
        Subscription subscription,
        CancellationToken cancellationToken)
    {
        var subscriptionQueryParameterName = subscription.Type switch
        {
            SubscriptionType.Artist => "artist_id",
            SubscriptionType.Label => "label_id",
            _ => throw new NotSupportedException($"Subscription type {subscription.Type} is not supported")
        };

        var queryParameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            [subscriptionQueryParameterName] = subscription.BeatportId.ToString(),
            ["publish_date"] = FormattableString.Invariant($"{subscription.RefreshedAt ?? clock.UtcToday.AddDays(-InitialPullDaysCount):yyyy-MM-dd}:{clock.UtcToday:yyyy-MM-dd}"),
            ["per_page"] = PageSize.ToString(CultureInfo.InvariantCulture),
        };
        var idsResult = await beatportClient.SearchForIdsAsync<BeatportReleaseDto>(queryParameters, cancellationToken);

        return idsResult.IsFailed
            ? Result.Fail(idsResult)
            : idsResult.Value.Select(BeatportId.Create).ToList().AsReadOnly();
    }

    private async Task<Result<IReadOnlyCollection<BeatportTrackDto>>> CollectTracksAsync(
        IEnumerable<int> trackIds,
        CancellationToken cancellationToken)
    {
        List<BeatportTrackDto> tracks = [];

        foreach (var trackId in trackIds)
        {
            var beatportTrackResult = await beatportClient.GetAsync<BeatportTrackDto>(trackId, cancellationToken);
            if (beatportTrackResult.IsFailed)
            {
                return Result.Fail(beatportTrackResult);
            }

            tracks.Add(beatportTrackResult.Value!);
        }

        return tracks;
    }

    private async Task<Result> ProcessSubscriptionsAsync(
        BeatportReleaseDto beatportRelease,
        IReadOnlyCollection<BeatportTrackDto> beatportTracks,
        CancellationToken cancellationToken)
    {
        var subscriptionTargets = new HashSet<(SubscriptionType Type, BeatportId BeatportId)>
        {
            (SubscriptionType.Label, BeatportId.Create(beatportRelease.Label.Id))
        };
        subscriptionTargets.UnionWith(beatportRelease.Artists.Select(artist => (SubscriptionType.Artist, BeatportId.Create(artist.Id))));
        subscriptionTargets.UnionWith(beatportRelease.Remixers.Select(remixer => (SubscriptionType.Artist, BeatportId.Create(remixer.Id))));
        subscriptionTargets.UnionWith(beatportTracks.SelectMany(track => track.Artists).Select(artist => (SubscriptionType.Artist, BeatportId.Create(artist.Id))));
        subscriptionTargets.UnionWith(beatportTracks.SelectMany(track => track.Remixers).Select(remixer => (SubscriptionType.Artist, BeatportId.Create(remixer.Id))));

        foreach ((SubscriptionType type, BeatportId beatportId) in subscriptionTargets)
        {
            var result = await subscriptionService.EnsureSubscriptionExistsAsync(type, beatportId, cancellationToken);
            if (result.IsFailed)
            {
                return result.HasRetryableError
                    ? throw new RetryableBeatportRequestException(result.ErrorMessage)
                    : Result.Fail(result);
            }
        }

        return Result.Ok();
    }
}

internal static partial class CollectReleasesCommandLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Started")]
    public static partial void Started(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "All subscriptions are up to date")]
    public static partial void NoSubscriptions(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing subscription {SubscriptionId}")]
    public static partial void Processing(ILogger logger, SubscriptionId subscriptionId);

    [LoggerMessage(Level = LogLevel.Error)]
    public static partial void Failed(ILogger logger, Exception exception);
}
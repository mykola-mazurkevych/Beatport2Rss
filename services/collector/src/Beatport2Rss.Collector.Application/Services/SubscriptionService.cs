using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.Interfaces.Services;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

namespace Beatport2Rss.Collector.Application.Services;

internal sealed class SubscriptionService(
    IClock clock,
    IBeatportClient beatportClient,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IIntegrationEventOutbox integrationEventOutbox) :
    ISubscriptionService
{
    public async Task<Result> EnsureSubscriptionExistsAsync(
        SubscriptionType subscriptionType,
        BeatportId beatportId,
        CancellationToken cancellationToken = default)
    {
        if (await subscriptionCommandRepository.ExistsAsync(subscriptionType, beatportId, cancellationToken))
        {
            return Result.Ok();
        }

        var beatportSubscriptionResult = subscriptionType switch
        {
            SubscriptionType.Artist => await GetArtistSubscriptionAsync(beatportId.Value, cancellationToken),
            SubscriptionType.Label => await GetLabelSubscriptionAsync(beatportId.Value, cancellationToken),
            _ => Result.Unprocessable($"Subscription type '{subscriptionType}' is not supported")
        };

        if (beatportSubscriptionResult.IsFailed)
        {
            return Result.Fail(beatportSubscriptionResult);
        }

        var beatportSubscription = beatportSubscriptionResult.Value;
        var subscription = await subscriptionCommandRepository
            .AddAsync(
                Subscription.Create(
                    SubscriptionId.Create(Guid.CreateVersion7()),
                    createdAt: clock.UtcNow,
                    beatportSubscription.Type,
                    beatportId),
                cancellationToken);
        
        var subscriptionCreated = new SubscriptionCreated(
            Id: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            subscription.Id.Value,
            beatportSubscription.Type,
            beatportSubscription.Name,
            beatportSubscription.Id,
            beatportSubscription.Slug,
            beatportSubscription.ImageUri,
            beatportSubscription.Website,
            beatportSubscription.Bio);
        await integrationEventOutbox.EnqueueAsync(subscriptionCreated, cancellationToken);

        return Result.Ok();
    }

    private async Task<Result<BeatportSubscription>> GetArtistSubscriptionAsync(int beatportId, CancellationToken cancellationToken)
    {
        var artistResult = await beatportClient.GetAsync<BeatportArtistDto>(beatportId, cancellationToken);
        return artistResult switch
        {
            { IsFailed: true } => Result.Fail<BeatportSubscription>(artistResult.Errors),
            { Value: null } => Result.NotFound($"Artist {beatportId} not found"),
            _ => new BeatportSubscription(
                SubscriptionType.Artist,
                artistResult.Value.Id,
                artistResult.Value.Slug,
                artistResult.Value.Name,
                artistResult.Value.Image.Uri,
                artistResult.Value.Website,
                artistResult.Value.Bio)
        };
    }

    private async Task<Result<BeatportSubscription>> GetLabelSubscriptionAsync(int beatportId, CancellationToken cancellationToken)
    {
        var labelResult = await beatportClient.GetAsync<BeatportLabelDto>(beatportId, cancellationToken);
        return labelResult switch
        {
            { IsFailed: true } => Result.Fail<BeatportSubscription>(labelResult.Errors),
            { Value: null } => Result.NotFound($"Label {beatportId} not found"),
            _ => new BeatportSubscription(
                SubscriptionType.Label,
                labelResult.Value.Id,
                labelResult.Value.Slug,
                labelResult.Value.Name,
                labelResult.Value.Image.Uri,
                Website: null,
                Bio: null)
        };
    }

    private sealed record BeatportSubscription(
        SubscriptionType Type,
        int Id,
        string Slug,
        string Name,
        Uri ImageUri,
        Uri? Website,
        string? Bio);
}
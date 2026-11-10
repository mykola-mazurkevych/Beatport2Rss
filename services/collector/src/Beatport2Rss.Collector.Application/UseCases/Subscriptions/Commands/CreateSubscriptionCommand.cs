using Beatport2Rss.Collector.Application.Exceptions;
using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;

internal sealed record CreateSubscriptionCommand(
    SubscriptionType Type,
    int BeatportId);

internal sealed class CreateSubscriptionCommandHandler(
    IClock clock,
    IBeatportClient beatportClient,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork,
    IIntegrationEventOutbox integrationEventOutbox,
    ILogger<CreateSubscriptionCommand> logger)
{
    public async Task HandleAsync(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        CreateSubscriptionCommandLoggerMessages.Creating(logger, command.Type, command.BeatportId);

        var beatportSubscriptionResult = await GetBeatportSubscriptionAsync(command.Type, command.BeatportId, cancellationToken);
        if (beatportSubscriptionResult.IsFailed)
        {
            CreateSubscriptionCommandLoggerMessages.BeatportFailure(logger, beatportSubscriptionResult.ErrorMessage);

            if (beatportSubscriptionResult.Errors.Any(error => error is TransientError or TransientHttpError or RequestTimeoutError or TooManyRequestsError))
            {
                throw new RetryableBeatportRequestException(beatportSubscriptionResult.ErrorMessage);
            }

            return;
        }

        var beatportSubscription = beatportSubscriptionResult.Value;

        var beatportId = BeatportId.Create(beatportSubscription.Id);
        var subscription =
            await subscriptionCommandRepository.FindAsync(beatportSubscription.Type, beatportId, cancellationToken) ??
            await subscriptionCommandRepository
                .AddAsync(
                    Subscription.Create(
                        SubscriptionId.Create(Guid.CreateVersion7()),
                        createdAt: clock.UtcNow,
                        beatportSubscription.Type,
                        beatportId,
                        subscribersCount: 0,
                        refreshedAt: null),
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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        CreateSubscriptionCommandLoggerMessages.Created(logger, subscription.Id.Value);
    }

    private async Task<Result<BeatportSubscription>> GetBeatportSubscriptionAsync(
        SubscriptionType type,
        int beatportId,
        CancellationToken cancellationToken)
    {
        return type switch
        {
            SubscriptionType.Artist => await GetArtistSubscriptionAsync(beatportId, cancellationToken),
            SubscriptionType.Label => await GetLabelSubscriptionAsync(beatportId, cancellationToken),
            _ => Result.Unprocessable($"Subscription type '{type}' is not supported")
        };
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

internal static partial class CreateSubscriptionCommandLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating {SubscriptionType} subscription {BeatportId}")]
    public static partial void Creating(ILogger logger, SubscriptionType subscriptionType, int beatportId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to get subscription details from Beatport: {ErrorMessage}")]
    public static partial void BeatportFailure(ILogger logger, string errorMessage);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscription created: {SubscriptionId}")]
    public static partial void Created(ILogger logger, Guid subscriptionId);
}
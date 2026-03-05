using Beatport2Rss.Application.Dtos.Beatport;
using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionCommand(
    int BeatportId,
    BeatportSubscriptionType BeatportType) :
    ICommand<Result<SubscriptionDto>>;

internal sealed class CreateSubscriptionCommandValidator :
    AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(c => c.BeatportId).GreaterThan(0);
        RuleFor(c => c.BeatportType).IsInEnum();
    }
}

internal sealed class CreateSubscriptionCommandHandler(
    IBeatportClient beatportClient,
    IBeatportUriBuilder beatportUriBuilder,
    IClock clock,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    ITokenQueryRepository tokenQueryRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateSubscriptionCommand, Result<SubscriptionDto>>
{
    public async ValueTask<Result<SubscriptionDto>> Handle(
        CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var beatportId = BeatportId.Create(command.BeatportId);
        if (await subscriptionCommandRepository.ExistsAsync(s => s.BeatportType == command.BeatportType && s.BeatportId == beatportId, cancellationToken))
        {
            return Result.Conflict($"{command.BeatportType} already exists.");
        }

        var token = await tokenQueryRepository.FindAsync(cancellationToken);
        if (token is null)
        {
            return Result.Unprocessable("Token is missing.");
        }

        var subscriptionResult = command.BeatportType switch
        {
            BeatportSubscriptionType.Artist => await GetArtistSubscriptionAsync(beatportId, token.AccessToken, cancellationToken),
            BeatportSubscriptionType.Label => await GetLabelSubscriptionAsync(beatportId, token.AccessToken, cancellationToken),
            _ => Result.Unprocessable($"{command.BeatportType} is not supported.")
        };

        if (subscriptionResult.IsFailed)
        {
            return Result.Unprocessable(subscriptionResult);
        }

        var subscription = await subscriptionCommandRepository.AddAsync(subscriptionResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubscriptionDto(
            subscription.Id,
            subscription.Name,
            subscription.BeatportId,
            subscription.BeatportSlug,
            beatportUriBuilder.Build(subscription.BeatportType, subscription.BeatportId, subscription.BeatportSlug),
            subscription.ImageUri,
            subscription.CreatedAt,
            subscription.RefreshedAt);
    }

    private async Task<Result<Subscription>> GetArtistSubscriptionAsync(
        BeatportId beatportId,
        BeatportAccessToken beatportAccessToken,
        CancellationToken cancellationToken)
    {
        Result<BeatportArtistDto?> artistResult = await beatportClient.GetAsync<BeatportArtistDto>(beatportId, beatportAccessToken, cancellationToken);
        return artistResult switch
        {
            { IsFailed: true } => Result.Unprocessable(artistResult),
            { Value: null } => Result.Unprocessable("Not Found."),
            _ => Subscription.Create(
                clock.UtcNow,
                BeatportSubscriptionType.Artist,
                artistResult.Value.Id,
                artistResult.Value.Slug,
                artistResult.Value.Name,
                artistResult.Value.Image.Uri,
                refreshedAt: null)
        };
    }

    private async Task<Result<Subscription>> GetLabelSubscriptionAsync(
        BeatportId beatportId,
        BeatportAccessToken beatportAccessToken,
        CancellationToken cancellationToken)
    {
        var labelResult = await beatportClient.GetAsync<BeatportLabelDto>(beatportId, beatportAccessToken, cancellationToken);
        return labelResult switch
        {
            { IsFailed: true } => Result.Unprocessable(labelResult.Errors[0].Message),
            { Value: null } => Result.Unprocessable("Not Found."),
            _ => Subscription.Create(
                clock.UtcNow,
                BeatportSubscriptionType.Label,
                labelResult.Value.Id,
                labelResult.Value.Slug,
                labelResult.Value.Name,
                labelResult.Value.Image.Uri,
                refreshedAt: null)
        };
    }
}
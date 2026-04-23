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
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId) :
    ICommand<Result<SubscriptionDto>>;

internal sealed class CreateSubscriptionCommandValidator :
    AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(c => c.BeatportType).IsInEnum();
    }
}

internal sealed class CreateSubscriptionCommandHandler(
    IBeatportClient beatportClient,
    IBeatportUriBuilder beatportUriBuilder,
    IClock clock,
    ISlugGenerator slugGenerator,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    ITokenQueryRepository tokenQueryRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateSubscriptionCommand, Result<SubscriptionDto>>
{
    public async ValueTask<Result<SubscriptionDto>> Handle(
        CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (await subscriptionCommandRepository.ExistsAsync(command.BeatportType, command.BeatportId, cancellationToken))
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
            BeatportSubscriptionType.Artist => await GetArtistSubscriptionAsync(command.BeatportId, token.AccessToken, cancellationToken),
            BeatportSubscriptionType.Label => await GetLabelSubscriptionAsync(command.BeatportId, token.AccessToken, cancellationToken),
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
            subscription.Slug,
            subscription.BeatportType,
            beatportUriBuilder.Build(
                subscription.BeatportType,
                subscription.BeatportId,
                subscription.BeatportSlug),
            subscription.ImageUri,
            [],
            subscription.CreatedAt,
            subscription.RefreshedAt);
    }

    private async Task<Result<Subscription>> GetArtistSubscriptionAsync(
        BeatportId beatportId,
        BeatportAccessToken beatportAccessToken,
        CancellationToken cancellationToken)
    {
        var artistResult = await beatportClient.GetAsync<BeatportArtistDto>(beatportId, beatportAccessToken, cancellationToken);
        return artistResult switch
        {
            { IsFailed: true } => Result.Unprocessable(artistResult),
            { Value: null } => Result.Unprocessable("Not found."),
            _ => Subscription.Create(
                clock.UtcNow,
                SubscriptionName.Create(artistResult.Value.Name),
                slugGenerator.Generate(artistResult.Value.Name),
                BeatportSubscriptionType.Artist,
                artistResult.Value.Id,
                artistResult.Value.Slug,
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
            { Value: null } => Result.Unprocessable("Not found."),
            _ => Subscription.Create(
                clock.UtcNow,
                SubscriptionName.Create(labelResult.Value.Name),
                slugGenerator.Generate(labelResult.Value.Name),
                BeatportSubscriptionType.Label,
                labelResult.Value.Id,
                labelResult.Value.Slug,
                labelResult.Value.Image.Uri,
                refreshedAt: null)
        };
    }
}
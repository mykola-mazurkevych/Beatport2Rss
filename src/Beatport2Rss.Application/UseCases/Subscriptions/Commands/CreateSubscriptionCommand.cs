using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Common.Beatport;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.BeatportTokenProvider.Services.Interfaces;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Countries;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionCommand(
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    string? CountryCode) :
    ICommand<Result<SubscriptionDto>>;

internal sealed class CreateSubscriptionCommandValidator :
    AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(c => c.BeatportType).IsInEnum();
        RuleFor(c => c.CountryCode).IsCountryCode();
    }
}

internal sealed class CreateSubscriptionCommandHandler(
    IBeatportClient beatportClient,
    IBeatportUriBuilder beatportUriBuilder,
    IClock clock,
    ISlugGenerator slugGenerator,
    IBeatportTokenProvider tokenProvider,
    ISubscriptionCommandRepository subscriptionCommandRepository,
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

        var tokenResult = await tokenProvider.ProvideAsync(cancellationToken);
        if (tokenResult.IsFailed)
        {
            return Result.Unprocessable(tokenResult);
        }

        var accessToken = tokenResult.Value;

        var countryCode = string.IsNullOrEmpty(command.CountryCode)
            ? (CountryCode?)null
            : CountryCode.Create(command.CountryCode);

        var subscriptionResult = command.BeatportType switch
        {
            BeatportSubscriptionType.Artist => await GetArtistSubscriptionAsync(
                command.BeatportId,
                accessToken,
                countryCode,
                cancellationToken),
            BeatportSubscriptionType.Label => await GetLabelSubscriptionAsync(
                command.BeatportId,
                accessToken,
                countryCode,
                cancellationToken),
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
                    subscription.BeatportId.Value,
                    subscription.BeatportSlug.Value),
            subscription.ImageUri,
            Country: null,
            Tags: []);
    }

    private async Task<Result<Subscription>> GetArtistSubscriptionAsync(
        BeatportId beatportId,
        string accessToken,
        CountryCode? countryCode,
        CancellationToken cancellationToken)
    {
        var artistResult = await beatportClient.GetAsync<BeatportArtistDto>(beatportId.Value, accessToken, cancellationToken);
        return artistResult switch
        {
            { IsFailed: true } => Result.Unprocessable(artistResult),
            { Value: null } => Result.Unprocessable("Not found."),
            _ => Subscription.Create(
                clock.UtcNow,
                SubscriptionName.Create(artistResult.Value.Name),
                slugGenerator.Generate(artistResult.Value.Name),
                BeatportSubscriptionType.Artist,
                BeatportId.Create(artistResult.Value.Id),
                BeatportSlug.Create(artistResult.Value.Slug),
                artistResult.Value.Image.Uri,
                countryCode)
        };
    }

    private async Task<Result<Subscription>> GetLabelSubscriptionAsync(
        BeatportId beatportId,
        string accessToken,
        CountryCode? countryCode,
        CancellationToken cancellationToken)
    {
        var labelResult = await beatportClient.GetAsync<BeatportLabelDto>(beatportId.Value, accessToken, cancellationToken);
        return labelResult switch
        {
            { IsFailed: true } => Result.Unprocessable(labelResult.Errors[0].Message),
            { Value: null } => Result.Unprocessable("Not found."),
            _ => Subscription.Create(
                clock.UtcNow,
                SubscriptionName.Create(labelResult.Value.Name),
                slugGenerator.Generate(labelResult.Value.Name),
                BeatportSubscriptionType.Label,
                BeatportId.Create(labelResult.Value.Id),
                BeatportSlug.Create(labelResult.Value.Slug),
                labelResult.Value.Image.Uri,
                countryCode)
        };
    }
}
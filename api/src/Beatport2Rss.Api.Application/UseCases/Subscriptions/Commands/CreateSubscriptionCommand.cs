using Beatport2Rss.Api.Application.Dtos.Subscriptions;
using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.BeatportTokenProvider.Services.Interfaces;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionCommand(
    SubscriptionType Type,
    BeatportId BeatportId,
    string? CountryCode) :
    ICommand<Result<SubscriptionDto>>;

internal sealed class CreateSubscriptionCommandValidator :
    AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(c => c.Type).IsInEnum();
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
        if (await subscriptionCommandRepository.ExistsAsync(command.Type, command.BeatportId, cancellationToken))
        {
            return Result.Conflict($"{command.Type} already exists.");
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

        var subscriptionResult = command.Type switch
        {
            SubscriptionType.Artist => await GetArtistSubscriptionAsync(
                command.BeatportId,
                accessToken,
                countryCode,
                cancellationToken),
            SubscriptionType.Label => await GetLabelSubscriptionAsync(
                command.BeatportId,
                accessToken,
                countryCode,
                cancellationToken),
            _ => Result.Unprocessable($"{command.Type} is not supported.")
        };

        if (subscriptionResult.IsFailed)
        {
            return Result.Unprocessable(subscriptionResult);
        }

        var subscription = await subscriptionCommandRepository.AddAsync(subscriptionResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubscriptionDto(
            subscription.Id,
            subscription.Type,
            subscription.Name,
            subscription.Slug,
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
                SubscriptionId.Create(Guid.CreateVersion7()),
                clock.UtcNow,
                SubscriptionType.Artist,
                SubscriptionName.Create(artistResult.Value.Name),
                slugGenerator.Generate(artistResult.Value.Name),
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
                SubscriptionId.Create(Guid.CreateVersion7()),
                clock.UtcNow,
                SubscriptionType.Label,
                SubscriptionName.Create(labelResult.Value.Name),
                slugGenerator.Generate(labelResult.Value.Name),
                BeatportId.Create(labelResult.Value.Id),
                BeatportSlug.Create(labelResult.Value.Slug),
                labelResult.Value.Image.Uri,
                countryCode)
        };
    }
}
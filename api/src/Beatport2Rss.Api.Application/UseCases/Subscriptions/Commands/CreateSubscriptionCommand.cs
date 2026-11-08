using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;

internal sealed record CreateSubscriptionCommand(
    Guid SubscriptionId,
    SubscriptionType Type,
    string Name,
    int BeatportId,
    string BeatportSlug,
    Uri ImageUri,
    Uri? Website,
    string? Bio);

internal sealed class CreateSubscriptionCommandHandler(
    IClock clock,
    ISlugGenerator slugGenerator,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        if (await subscriptionCommandRepository.ExistsAsync(subscriptionId, cancellationToken))
        {
            // log warning
            return;
        }

        await subscriptionCommandRepository.AddAsync(
            Subscription.Create(
                subscriptionId,
                createdAt: clock.UtcNow,
                command.Type,
                SubscriptionName.Create(command.Name),
                slugGenerator.Generate(command.Name),
                BeatportId.Create(command.BeatportId),
                BeatportSlug.Create(command.BeatportSlug),
                command.ImageUri,
                command.Website,
                countryCode: null,
                command.Bio),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Interfaces;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions;

public sealed record CreateSubscriptionCommand(
    Guid SubscriptionId,
    int Type,
    int BeatportId,
    string BeatportSlug) :
    ICommand<Result>;

internal sealed class CreateSubscriptionHandler(
    IClock clock,
    IUnitOfWork unitOfWork,
    ISubscriptionCommandRepository subscriptionCommandRepository) :
    ICommandHandler<CreateSubscriptionCommand, Result>
{
    public async ValueTask<Result> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        if (await subscriptionCommandRepository.ExistsAsync(subscriptionId, cancellationToken))
        {
            return Result.Conflict($"Subscription with id {subscriptionId} already exists.");
        }

        var subscription = Subscription.Create(
            subscriptionId,
            clock.UtcNow,
            (SubscriptionType)command.Type,
            BeatportId.Create(command.BeatportId),
            BeatportSlug.Create(command.BeatportSlug),
            subscribersCount: 0,
            refreshedAt: null);

        await subscriptionCommandRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
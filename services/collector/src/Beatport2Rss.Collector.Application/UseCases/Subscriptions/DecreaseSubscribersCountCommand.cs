using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions;

public sealed record DecreaseSubscribersCountCommand(
    Guid SubscriptionId) :
    ICommand<Result>;

internal sealed class DecreaseSubscribersCountCommandHandler(
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DecreaseSubscribersCountCommand, Result>
{
    public async ValueTask<Result> Handle(DecreaseSubscribersCountCommand command, CancellationToken cancellationToken)
    {
        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        var subscription = await subscriptionCommandRepository.FindAsync(subscriptionId, cancellationToken);
        if (subscription is null)
        {
            return Result.NotFound($"Subscription with id {subscriptionId} not found.");
        }

        subscription.DecreaseSubscribersCount();
        subscriptionCommandRepository.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
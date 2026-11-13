using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;

public sealed record DeleteSubscriptionTagCommand(
    Guid SubscriptionId,
    Guid TagId);

public sealed class DeleteSubscriptionTagCommandHandler(
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        DeleteSubscriptionTagCommand command,
        CancellationToken cancellationToken)
    {
        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        var subscription = await subscriptionCommandRepository.FindAsync(subscriptionId, cancellationToken);
        if (subscription is null)
        {
            return;
        }

        var tagId = TagId.Create(command.TagId);
        if (!subscription.ContainsTag(tagId))
        {
            return;
        }

        subscription.RemoveTag(tagId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

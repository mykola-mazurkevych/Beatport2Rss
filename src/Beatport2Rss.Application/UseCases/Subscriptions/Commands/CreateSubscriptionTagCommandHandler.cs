using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionTagCommand(
    UserId UserId,
    Slug SubscriptionSlug,
    Slug TagSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireSubscription, IRequireTag;

internal sealed class CreateSubscriptionTagCommandHandler(
    ITagQueryRepository tagQueryRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateSubscriptionTagCommand, Result>
{
    public async ValueTask<Result> Handle(
        CreateSubscriptionTagCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionCommandRepository.LoadWithTagsAsync(command.SubscriptionSlug, cancellationToken);
        var tagId = await tagQueryRepository.LoadTagIdAsync(command.UserId, command.TagSlug, cancellationToken);

        if (subscription.ContainsTag(tagId))
        {
            return Result.Conflict("Tag already exists in the subscription.");
        }

        subscription.AddTag(tagId);

        subscriptionCommandRepository.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
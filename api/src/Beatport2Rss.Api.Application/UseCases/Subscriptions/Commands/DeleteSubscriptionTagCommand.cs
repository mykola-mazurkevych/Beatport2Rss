using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;

public sealed record DeleteSubscriptionTagCommand(
    UserId UserId,
    Slug SubscriptionSlug,
    Slug TagSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireSubscription, IRequireTag;

internal sealed class DeleteSubscriptionTagCommandHandler(
    ITagQueryRepository tagQueryRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSubscriptionTagCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteSubscriptionTagCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionCommandRepository.LoadWithTagsAsync(command.SubscriptionSlug, cancellationToken);
        var tagId = await tagQueryRepository.LoadTagIdAsync(command.UserId, command.TagSlug, cancellationToken);

        if (!subscription.ContainsTag(tagId))
        {
            return Result.NotFound("The subscription does not contain the tag.");
        }

        subscription.RemoveTag(tagId);

        subscriptionCommandRepository.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
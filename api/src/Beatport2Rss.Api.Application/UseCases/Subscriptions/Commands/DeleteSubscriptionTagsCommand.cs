using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;

public sealed record DeleteSubscriptionTagsCommand(
    UserId UserId,
    Slug SubscriptionSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireSubscription;

internal sealed class DeleteSubscriptionTagsCommandHandler(
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSubscriptionTagsCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteSubscriptionTagsCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionCommandRepository.LoadWithTagsAsync(command.SubscriptionSlug, cancellationToken);

        subscription.RemoveTags();

        subscriptionCommandRepository.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
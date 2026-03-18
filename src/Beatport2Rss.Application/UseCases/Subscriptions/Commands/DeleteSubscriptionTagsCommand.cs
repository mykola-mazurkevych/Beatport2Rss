using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Commands;

public sealed record DeleteSubscriptionTagsCommand(
    UserId UserId,
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug) :
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
        var subscription = await subscriptionCommandRepository.LoadWithTagsAsync(
            command.BeatportType,
            command.BeatportId,
            command.BeatportSlug,
            cancellationToken);

        subscription.RemoveTags();

        subscriptionCommandRepository.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
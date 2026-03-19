using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Subscriptions.Commands;

public sealed record DeleteSubscriptionTagCommand(
    UserId UserId,
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
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
        var tagId = await tagQueryRepository.LoadTagIdAsync(command.UserId, command.TagSlug, cancellationToken);
        var subscription = await subscriptionCommandRepository.LoadWithTagsAsync(
            command.BeatportType,
            command.BeatportId,
            command.BeatportSlug,
            cancellationToken);

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
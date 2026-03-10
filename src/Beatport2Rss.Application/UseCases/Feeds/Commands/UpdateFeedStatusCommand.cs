using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public sealed record UpdateFeedStatusCommand(
    UserId UserId,
    Slug FeedSlug,
    bool IsActive) :
    ICommand<Result>, IRequireActiveUser, IRequireFeed;

internal sealed class UpdateFeedStatusCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateFeedStatusCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateFeedStatusCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadAsync(f => f.UserId == command.UserId && f.Slug == command.FeedSlug, cancellationToken);

        feed.UpdateStatus(command.IsActive);

        feedCommandRepository.Update(feed);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public sealed record DeleteFeedCommand(
    UserId UserId,
    Slug FeedSlug) :
    ICommand<Result>, IRequireUser, IRequireFeed;

internal sealed class DeleteFeedCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteFeedCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteFeedCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadAsync(f => f.UserId == command.UserId && f.Slug == command.FeedSlug, cancellationToken);

        feedCommandRepository.Delete(feed);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
using Beatport2Rss.Application.Extensions;
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
    Slug Slug) :
    ICommand<Result>, IRequireSlug;

internal sealed class DeleteFeedCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteFeedCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteFeedCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadWithFeedsAsync(command.UserId, cancellationToken);

        // TODO: Move to behavior
        if (!user.HasFeed(command.Slug))
        {
            return Result.NotFound($"Feed with slug '{command.Slug}' was not found.");
        }

        user.RemoveFeed(command.Slug);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public sealed record UpdateFeedStatusRequest(
    bool IsActive);

public sealed record UpdateFeedStatusCommand(
    UserId UserId,
    Slug Slug,
    bool IsActive) :
    ICommand<Result>, IRequireActiveUser, IRequireSlug;

internal sealed class UpdateFeedStatusCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateFeedStatusCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateFeedStatusCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadWithFeedsAsync(command.UserId, cancellationToken);

        // TODO: Move to behavior
        if (!user.HasFeed(command.Slug))
        {
            return Result.NotFound($"Feed with slug '{command.Slug}' was not found.");
        }

        user.UpdateFeedStatus(command.Slug, command.IsActive);

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
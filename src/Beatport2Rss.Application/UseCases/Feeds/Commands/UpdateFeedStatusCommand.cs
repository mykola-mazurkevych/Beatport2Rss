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
    Slug Slug,
    bool IsActive) :
    ICommand<Result>, IRequireActiveUser, IRequireFeed;

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

        user.UpdateFeedStatus(command.Slug, command.IsActive);

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
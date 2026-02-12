using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tags.Commands;

public sealed record DeleteTagCommand(
    UserId UserId,
    Slug Slug) :
    ICommand<Result>, IRequireUser, IRequireTag;

internal sealed class DeleteTagCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteTagCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteTagCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadWithTagsAsync(command.UserId, cancellationToken);

        user.RemoveTag(command.Slug);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
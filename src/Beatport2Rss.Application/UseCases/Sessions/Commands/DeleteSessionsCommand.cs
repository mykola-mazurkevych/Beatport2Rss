using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public sealed record DeleteSessionsCommand(
    UserId UserId) :
    ICommand<Result>;

internal sealed class DeleteSessionsCommandHandler(
    ISessionCommandRepository sessionCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSessionsCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteSessionsCommand command, CancellationToken cancellationToken = default)
    {
        var sessions = await sessionCommandRepository.FindAllAsync(s => s.UserId == command.UserId, cancellationToken);

        sessionCommandRepository.DeleteRange(sessions);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
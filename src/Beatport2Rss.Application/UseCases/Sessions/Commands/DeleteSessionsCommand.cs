using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public sealed record DeleteSessionsCommand(
    UserId UserId) :
    ICommand<Result>;

internal sealed class DeleteSessionsCommandHandler(
    ISessionCommandRepository sessionCommandRepository) :
    ICommandHandler<DeleteSessionsCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteSessionsCommand command, CancellationToken cancellationToken = default)
    {
        await sessionCommandRepository.DeleteAsync(s => s.UserId == command.UserId, cancellationToken);

        return Result.Ok();
    }
}
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Sessions.Commands;

public sealed record DeleteExpiredSessionsCommand :
    ICommand<Result>;

internal sealed class DeleteExpiredSessionsCommandHandler(
    ISessionCommandRepository sessionCommandRepository) :
    ICommandHandler<DeleteExpiredSessionsCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteExpiredSessionsCommand command,
        CancellationToken cancellationToken = default)
    {
        await sessionCommandRepository.DeleteExpiredAsync(cancellationToken);

        return Result.Ok();
    }
}
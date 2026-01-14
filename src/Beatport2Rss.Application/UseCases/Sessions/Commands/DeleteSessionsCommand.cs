using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct DeleteSessionsCommand(Guid UserId) :
    ICommand<Result>;

public sealed class DeleteSessionsCommandValidator :
    AbstractValidator<DeleteSessionsCommand>
{
    public DeleteSessionsCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}

public sealed class DeleteSessionsCommandHandler(
    ISessionCommandRepository sessionRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSessionsCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteSessionsCommand command, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(command.UserId);
        var sessions = await sessionRepository.FindAllAsync(s => s.UserId == userId, cancellationToken);

        sessionRepository.DeleteRange(sessions);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
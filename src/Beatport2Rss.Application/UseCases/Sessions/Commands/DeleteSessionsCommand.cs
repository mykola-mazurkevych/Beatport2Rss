using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using ErrorOr;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct DeleteSessionsCommand(Guid UserId) :
    ICommand<ErrorOr<Success>>, IValidate;

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
    ICommandHandler<DeleteSessionsCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(DeleteSessionsCommand command, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(command.UserId);
        var sessions = await sessionRepository.FindAllAsync(s => s.UserId == userId, cancellationToken);

        sessionRepository.DeleteRange(sessions);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
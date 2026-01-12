using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

using ErrorOr;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct DeleteSessionCommand(Guid SessionId) :
    ICommand<ErrorOr<Success>>, IValidate;

public sealed class DeleteSessionCommandValidator :
    AbstractValidator<DeleteSessionCommand>
{
    public DeleteSessionCommandValidator()
    {
        RuleFor(c => c.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}

public sealed class DeleteSessionCommandHandler(
    ISessionCommandRepository sessionRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSessionCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(
        DeleteSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var sessionId = SessionId.Create(command.SessionId);
        var session = await sessionRepository.LoadAsync(sessionId, cancellationToken);

        sessionRepository.Delete(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
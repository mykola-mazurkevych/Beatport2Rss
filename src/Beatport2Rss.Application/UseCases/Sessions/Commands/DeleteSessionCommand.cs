using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Types;
using Beatport2Rss.Domain.Sessions;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct DeleteSessionCommand(Guid SessionId);

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
    IValidator<DeleteSessionCommand> validator,
    ISessionQueryRepository sessionQueryRepository,
    ISessionCommandRepository sessionCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<OneOf<Success, ValidationFailed>> HandleAsync(
        DeleteSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var sessionId = SessionId.Create(command.SessionId);
        var session = (await sessionQueryRepository.GetAsync(sessionId, cancellationToken))!;

        sessionCommandRepository.Delete(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
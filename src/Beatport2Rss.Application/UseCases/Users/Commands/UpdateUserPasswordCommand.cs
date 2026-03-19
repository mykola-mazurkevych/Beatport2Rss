using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public sealed record UpdateUserPasswordCommand(
    UserId UserId,
    string Password) :
    ICommand<Result>, IRequireUser;

internal sealed class UpdateUserPasswordCommandValidator :
    AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(c => c.Password).IsPassword();
    }
}

internal sealed class UpdateUserPasswordCommandHandler(
    IPasswordHasher passwordHasher,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserPasswordCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateUserPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var password = Password.Create(command.Password);
        var passwordHash = passwordHasher.Hash(password);

        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken);

        user.UpdatePasswordHash(passwordHash);

        // TODO: delete all sessions?

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
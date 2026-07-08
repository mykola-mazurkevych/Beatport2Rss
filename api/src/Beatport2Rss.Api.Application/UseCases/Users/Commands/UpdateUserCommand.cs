using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Users.Commands;

public sealed record UpdateUserCommand(
    UserId UserId,
    string? FirstName,
    string? LastName,
    string? CountryCode) :
    ICommand<Result>, IRequireUser;

internal sealed class UpdateUserCommandValidator :
    AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.FirstName).IsFirstName();
        RuleFor(c => c.LastName).IsLastName();
        RuleFor(c => c.CountryCode).IsCountryCode();
    }
}

internal sealed class UpdateUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken);

        user.UpdateFirstName(command.FirstName);
        user.UpdateLastName(command.LastName);
        user.UpdateCountry(
            string.IsNullOrEmpty(command.CountryCode)
                ? null
                : CountryCode.Create(command.CountryCode));

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
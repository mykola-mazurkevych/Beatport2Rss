#pragma warning disable CA1863

using System.Globalization;

using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

file static class ErrorMessages
{
    public const string Forbidden = "The user is not active to perform this action.";
    public const string NotSupported = "User status '{0}' is not supported.";
}

internal abstract class UserValidationBehavior<TMessage, TResult>(
    IUserQueryRepository userQueryRepository)
    where TMessage : IMessage, IRequireActiveUser
    where TResult : Result
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var readModel = await userQueryRepository.LoadUserStatusReadModelAsync(userId, cancellationToken);

        return readModel.Status switch
        {
            UserStatus.Pending or UserStatus.Inactive => (TResult)Result.Forbidden(ErrorMessages.Forbidden),
            UserStatus.Active => await next(message, cancellationToken),
            _ => throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, ErrorMessages.NotSupported, readModel.Status))
        };
    }
}

internal abstract class UserValidationBehavior<TMessage, TResult, TResponse>(
    IUserQueryRepository userQueryRepository)
    where TMessage : IMessage, IRequireActiveUser
    where TResult : Result<TResponse>
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var readModel = await userQueryRepository.LoadUserStatusReadModelAsync(userId, cancellationToken);

        return readModel.Status switch
        {
            UserStatus.Pending or UserStatus.Inactive => (TResult)Result.Forbidden(ErrorMessages.Forbidden),
            UserStatus.Active => await next(message, cancellationToken),
            _ => throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, ErrorMessages.NotSupported, readModel.Status))
        };
    }
}
using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

// TODO: split into two behaviors to check if user exists and if it's active
public sealed class UserValidationBehavior<TMessage, TResponse>(
    IUserQueryRepository userQueryRepository) :
    IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage, IRequireActiveUser
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var userStatus = await userQueryRepository.LoadUserStatusQueryModelAsync(userId, cancellationToken);

        return userStatus switch
        {
            null => (TResponse)Result.Unauthorized("The user is not authorized to perform this action."),
            { Status: UserStatus.Pending or UserStatus.Inactive } => (TResponse)Result.Forbidden("The user is not active to perform this action."),
            { Status: UserStatus.Active } => await next(message, cancellationToken),
            _ => throw new NotSupportedException($"User status '{userStatus.Status}' is not supported.")
        };
    }
}
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

public sealed class ForbiddenBehavior<TMessage, TResponse>(IUserQueryRepository userRepository) :
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
        var user = await userRepository.FindAsync(userId, cancellationToken);

        return user is null || !user.IsActive
            ? (TResponse)Result.Forbidden("The user is not active to perform this action.")
            : await next(message, cancellationToken);
    }
}
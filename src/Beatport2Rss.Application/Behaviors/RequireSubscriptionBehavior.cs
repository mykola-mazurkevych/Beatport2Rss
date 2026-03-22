using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

file static class ErrorMessages
{
    public const string NotFound = "Subscription with slug '{0}' was not found.";
}

internal abstract class RequireSubscriptionBehavior<TMessage, TResult>(
    ISubscriptionQueryRepository subscriptionQueryRepository)
    where TMessage : IMessage, IRequireSubscription
    where TResult : Result
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var exists = await subscriptionQueryRepository.ExistsAsync(message.SubscriptionSlug, cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResult)Result.NotFound(ErrorMessages.NotFound, message.SubscriptionSlug);
    }
}

internal abstract class RequireSubscriptionBehavior<TMessage, TResult, TResponse>(
    ISubscriptionQueryRepository subscriptionQueryRepository)
    where TMessage : IMessage, IRequireSubscription
    where TResult : Result<TResponse>
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var exists = await subscriptionQueryRepository.ExistsAsync(message.SubscriptionSlug, cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResult)Result.NotFound(ErrorMessages.NotFound, message.SubscriptionSlug);
    }
}
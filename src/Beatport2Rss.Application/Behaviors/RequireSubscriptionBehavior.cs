using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

file static class ErrorMessages
{
    public const string NotFound = "Subscription of type '{0}' with id '{1}' and slug '{2}' was not found.";
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
        var exists = await subscriptionQueryRepository.ExistsAsync(
            message.BeatportType,
            message.BeatportId,
            message.BeatportSlug,
            cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResult)Result.NotFound(ErrorMessages.NotFound, message.BeatportType, message.BeatportId, message.BeatportSlug);
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
        var exists = await subscriptionQueryRepository.ExistsAsync(
            message.BeatportType,
            message.BeatportId,
            message.BeatportSlug,
            cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResult)Result.NotFound(ErrorMessages.NotFound, message.BeatportType, message.BeatportId, message.BeatportSlug);
    }
}
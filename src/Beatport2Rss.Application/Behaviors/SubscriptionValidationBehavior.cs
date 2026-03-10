using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

public sealed class SubscriptionValidationBehavior<TMessage, TResponse>(
    ISubscriptionQueryRepository subscriptionQueryRepository) :
    IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage, IRequireSubscription
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var exists = await subscriptionQueryRepository.ExistsAsync(message.BeatportType, message.BeatportId, message.BeatportSlug, cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResponse)Result.NotFound("Subscirption was not found.");
    }
}
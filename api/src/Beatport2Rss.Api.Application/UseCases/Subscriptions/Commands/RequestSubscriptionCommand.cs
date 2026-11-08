using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;

public sealed record RequestSubscriptionCommand(
    SubscriptionType Type,
    BeatportId BeatportId) :
    ICommand<Result>;

internal sealed class RequestSubscriptionCommandValidator :
    AbstractValidator<RequestSubscriptionCommand>
{
    public RequestSubscriptionCommandValidator()
    {
        RuleFor(c => c.Type).IsInEnum();
    }
}

internal sealed class RequestSubscriptionCommandHandler(
    IClock clock,
    IUnitOfWork unitOfWork,
    IIntegrationEventOutbox integrationEventOutbox) :
    ICommandHandler<RequestSubscriptionCommand, Result>
{
    public async ValueTask<Result> Handle(
        RequestSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscriptionRequested = new SubscriptionRequested(
            Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            command.Type,
            command.BeatportId.Value);
        await integrationEventOutbox.EnqueueAsync(subscriptionRequested, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
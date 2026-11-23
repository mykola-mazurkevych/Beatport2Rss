using Beatport2Rss.Common.IntegrationEvents;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.IntegrationEvents;

// TODO: why not putting it to the same project where other integration events live?
public sealed record EmailConfirmationCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid EmailConfirmationId) :
    IIntegrationEvent
{
    // TODO: why do we need this?
    public override string ToString() => nameof(EmailConfirmationCreated);
}
namespace Beatport2Rss.Common.IntegrationEvents.EmailConfirmations;

[IntegrationEventTopic("email.confirmation.created")]
public sealed record EmailConfirmationCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid EmailConfirmationId) :
    IIntegrationEvent;
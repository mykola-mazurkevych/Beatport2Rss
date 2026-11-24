namespace Beatport2Rss.Common.IntegrationEvents.EmailConfirmations;

[IntegrationEventTopic("email.confirmed")]
public sealed record EmailConfirmed(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid UserId,
    string EmailAddress) :
    IIntegrationEvent;

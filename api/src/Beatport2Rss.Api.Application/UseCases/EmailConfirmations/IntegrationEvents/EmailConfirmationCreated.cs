using Beatport2Rss.Common.IntegrationEvents;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.IntegrationEvents;

public sealed record EmailConfirmationCreated(Guid Id, DateTimeOffset OccurredAt, string Identifier) : IIntegrationEvent
{
    public override string ToString() => nameof(EmailConfirmationCreated);
}

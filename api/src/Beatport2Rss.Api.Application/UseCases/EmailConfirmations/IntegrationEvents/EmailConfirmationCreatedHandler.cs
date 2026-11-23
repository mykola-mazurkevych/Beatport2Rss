using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;
using Beatport2Rss.Common.IntegrationEvents.EmailConfirmations;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.IntegrationEvents;

public static class EmailConfirmationCreatedHandler
{
    public static SendConfirmationEmailCommand Handle(EmailConfirmationCreated message) =>
        new(message.EmailConfirmationId);
}
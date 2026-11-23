using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.IntegrationEvents;

public static class EmailConfirmationCreatedHandler
{
    public static SendConfirmationEmailCommand Handle(EmailConfirmationCreated message) =>
        new(message.EmailConfirmationId);
}
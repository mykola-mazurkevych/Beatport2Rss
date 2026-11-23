using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;
using Beatport2Rss.Common.IntegrationEvents.Users;

namespace Beatport2Rss.Api.Application.UseCases.Users.IntegrationEvents;

public static class UserCreatedHandler
{
    public static CreateEmailConfirmationCommand Handle(UserCreated message) => new(message.UserId);
}

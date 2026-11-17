using Beatport2Rss.Builder.Application.UseCases.Users.Commands;
using Beatport2Rss.Common.IntegrationEvents.Users;

namespace Beatport2Rss.Builder.Application.UseCases.Users.IntegrationEvents;

public static class UserCreatedHandler
{
    public static CreateUserCommand Handle(UserCreated userCreated) =>
        new(userCreated.UserId,
            userCreated.OccurredAt);
}
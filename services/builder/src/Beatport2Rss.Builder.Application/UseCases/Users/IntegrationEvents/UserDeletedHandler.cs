using Beatport2Rss.Builder.Application.UseCases.Users.Commands;
using Beatport2Rss.Common.IntegrationEvents.Users;

namespace Beatport2Rss.Builder.Application.UseCases.Users.IntegrationEvents;

public static class UserDeletedHandler
{
    public static DeleteUserCommand Handle(UserDeleted userDeleted) =>
        new(userDeleted.UserId);
}
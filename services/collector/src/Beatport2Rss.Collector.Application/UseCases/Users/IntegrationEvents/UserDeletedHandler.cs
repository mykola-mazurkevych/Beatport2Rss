using Beatport2Rss.Collector.Application.UseCases.Users.Commands;
using Beatport2Rss.Common.IntegrationEvents.Users;

namespace Beatport2Rss.Collector.Application.UseCases.Users.IntegrationEvents;

public static class UserDeletedHandler
{
    public static DeleteUserCommand Handle(UserDeleted userDeleted) =>
        new(userDeleted.UserId);
}
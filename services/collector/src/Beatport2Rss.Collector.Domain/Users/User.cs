using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Users;

namespace Beatport2Rss.Collector.Domain.Users;

public sealed class User :
    IAggregateRoot<UserId>
{
    private User()
    {
    }

    public UserId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static User Create(
        UserId id,
        DateTimeOffset createdAt) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
        };
}
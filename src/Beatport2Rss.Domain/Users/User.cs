using Beatport2Rss.SharedKernel;

namespace Beatport2Rss.Domain.Users;

public sealed class User : IAggregateRoot<UserId>
{
    ////private readonly HashSet<Feed> _feeds = [];
    ////private readonly HashSet<Tag> _tags = [];

    private User()
    {
    }

    public UserId Id { get; private set; }

    public Username Username { get; private set; }
    public string Slug { get; private set; } = null!;

    public EmailAddress EmailAddress { get; private set; }
    public PasswordHash PasswordHash { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    ////public IReadOnlySet<Feed> Feeds => _feeds.AsReadOnly();
    ////public IReadOnlySet<Tag> Tags => _tags.AsReadOnly();

    public static User Create(
        UserId id,
        Username username,
        string slug,
        PasswordHash passwordHash,
        EmailAddress emailAddress,
        DateTimeOffset createdDate) =>
        new()
        {
            Id = id,
            Username = username,
            Slug = slug,
            PasswordHash = passwordHash,
            EmailAddress = emailAddress,
            CreatedDate = createdDate,
        };
}
using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel;

namespace Beatport2Rss.Domain.Users;

public sealed class User : IAggregateRoot<UserId>
{
    private readonly HashSet<Feed> _feeds = [];
    private readonly HashSet<Tag> _tags = [];

    private User()
    {
    }

    public UserId Id { get; private set; }

    public Username Username { get; private set; }
    public Slug Slug { get; private set; }

    public EmailAddress EmailAddress { get; private set; }
    public PasswordHash PasswordHash { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    public IReadOnlySet<Feed> Feeds => _feeds.AsReadOnly();
    public IReadOnlySet<Tag> Tags => _tags.AsReadOnly();

    public static User Create(
        UserId id,
        Username username,
        Slug slug,
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
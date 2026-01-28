using Beatport2Rss.Domain.Common.Interfaces;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Domain.Users;

public sealed class User : IAggregateRoot<UserId>
{
    public const int NameLength = 100;

    private readonly HashSet<Feed> _feeds = [];
    private readonly HashSet<Tag> _tags = [];

    private User()
    {
    }

    public UserId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public EmailAddress EmailAddress { get; private set; }
    public PasswordHash PasswordHash { get; private set; }

    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }

    public UserStatus Status { get; private set; }

    public IReadOnlySet<Feed> Feeds => _feeds.AsReadOnly();
    public IReadOnlySet<Tag> Tags => _tags.AsReadOnly();

    public string? FullName =>
        string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? null
            : $"{FirstName} {LastName}".Trim();

    public bool IsActive => Status == UserStatus.Active;

    public static User Create(
        UserId id,
        DateTimeOffset createdAt,
        EmailAddress emailAddress,
        PasswordHash passwordHash,
        string? firstName,
        string? lastName,
        UserStatus status) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            EmailAddress = emailAddress,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Status = status,
        };

    public void AddFeed(Feed feed) =>
        _feeds.Add(feed);

    public void RemoveFeed(Feed feed) =>
        _feeds.RemoveWhere(f => f.Id == feed.Id);
}
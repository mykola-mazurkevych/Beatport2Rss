using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Users;

public sealed class User :
    IAggregateRoot<UserId>
{
    public const int NameLength = 100;

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

    public string? FullName =>
        string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? null
            : $"{FirstName} {LastName}".Trim();

    public bool IsActive =>
        Status == UserStatus.Active;

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

    public void UpdateEmailAddress(EmailAddress emailAddress) =>
        EmailAddress = emailAddress;

    public void UpdateFirstName(string? firstName) =>
        FirstName = firstName;

    public void UpdateLastName(string? lastName) =>
        LastName = lastName;

    public void UpdateStatus(bool isActive) =>
        Status = isActive ? UserStatus.Active : UserStatus.Inactive;
}
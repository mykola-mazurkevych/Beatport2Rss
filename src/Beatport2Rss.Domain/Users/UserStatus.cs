namespace Beatport2Rss.Domain.Users;

public enum UserStatus
{
    Pending,
    Active,
    Inactive,
    Deleted, // TODO: Decide if this status is needed
}
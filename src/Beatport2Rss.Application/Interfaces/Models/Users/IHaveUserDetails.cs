using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Models.Users;

public interface IHaveUserDetails
{
    EmailAddress EmailAddress { get; }
    string? FirstName { get; }
    string? LastName { get; }
    bool IsActive { get; }
    int FeedsCount { get; }
    int TagsCount { get; }
}
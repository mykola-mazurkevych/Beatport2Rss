using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Models.Users;

public interface IHaveUserAuthDetails
{
    UserId Id { get; }
    EmailAddress EmailAddress { get; }
    string? FirstName { get; }
    string? LastName { get; }
}
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Models.Users;

public interface IHaveUserAuth
{
    UserId Id { get; }
    EmailAddress EmailAddress { get; }
    PasswordHash PasswordHash { get; }
    string? FirstName { get; }
    string? LastName { get; }
}
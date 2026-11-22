using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Models;

public interface IUserAuthDetails
{
    UserId Id { get; }
    EmailAddress EmailAddress { get; }
    string? FirstName { get; }
    string? LastName { get; }
}
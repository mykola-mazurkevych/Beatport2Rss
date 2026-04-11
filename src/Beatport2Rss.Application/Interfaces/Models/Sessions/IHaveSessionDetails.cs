using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Models.Sessions;

public interface IHaveSessionDetails
{
    SessionId Id { get; }
    DateTimeOffset CreatedAt { get; }
    EmailAddress EmailAddress { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? UserAgent { get; }
    string? IpAddress { get; }
    bool IsExpired { get; }
}
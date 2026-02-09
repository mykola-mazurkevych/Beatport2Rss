using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Messages;

public interface IRequireUser
{
    UserId UserId { get; }
}
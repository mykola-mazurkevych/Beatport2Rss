using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Messages;

public interface IRequireUser
{
    UserId UserId { get; }
}
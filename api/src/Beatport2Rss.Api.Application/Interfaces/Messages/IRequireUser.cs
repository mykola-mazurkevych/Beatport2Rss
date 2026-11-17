using Beatport2Rss.Common.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Messages;

public interface IRequireUser
{
    UserId UserId { get; }
}
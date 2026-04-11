using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Models.Users;

public interface IHaveUserStatusDetails
{
    UserStatus Status { get; }
}
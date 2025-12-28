using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Services;

public interface IAccessTokenService
{
    (AccessToken AccessToken, int ExpiresIn) Generate(User user, SessionId sessionId);
}
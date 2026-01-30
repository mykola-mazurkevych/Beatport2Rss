using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Application.Interfaces.Services.Security;

public interface IAccessTokenService
{
    (AccessToken AccessToken, int ExpiresIn) Generate(UserAuthDetailsReadModel userAuthDetails, SessionId sessionId);
}
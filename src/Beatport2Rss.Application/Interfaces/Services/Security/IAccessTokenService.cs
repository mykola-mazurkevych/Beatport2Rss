using Beatport2Rss.Application.Interfaces.Models.Users;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Application.Interfaces.Services.Security;

public interface IAccessTokenService
{
    (AccessToken AccessToken, int ExpiresIn) Generate(IHaveUserAuth userAuth, SessionId sessionId);
}
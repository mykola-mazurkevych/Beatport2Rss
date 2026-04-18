using Beatport2Rss.Application.Interfaces.Models;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Application.Interfaces.Services.Security;

public interface IAccessTokenService
{
    (AccessToken AccessToken, int ExpiresIn) Generate(IUserAuthDetails userAuthDetails, SessionId sessionId);
}
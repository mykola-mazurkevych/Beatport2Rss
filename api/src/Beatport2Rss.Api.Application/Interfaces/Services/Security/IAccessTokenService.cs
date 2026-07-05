using Beatport2Rss.Api.Application.Interfaces.Models;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Sessions;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Security;

public interface IAccessTokenService
{
    (AccessToken AccessToken, int ExpiresIn) Generate(IUserAuthDetails userAuthDetails, SessionId sessionId);
}
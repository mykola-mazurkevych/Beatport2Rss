using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Beatport2Rss.Api.Domain.Sessions;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Extensions;

internal static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public SessionId SessionId =>
            SessionId.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sid) ?? string.Empty, provider: null);

        public UserId Id =>
            UserId.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty, provider: null);
    }
}
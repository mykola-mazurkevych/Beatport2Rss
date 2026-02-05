using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.WebApi.Extensions;

internal static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public Guid SessionId =>
            SessionId.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sid) ?? string.Empty, provider: null);

        public UserId Id =>
            UserId.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty, provider: null);
    }
}
#pragma warning disable CA1034 // Nested types should not be visible

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Beatport2Rss.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal user)
    {
        public Guid Id =>
            Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var sessionId)
                ? sessionId
                : Guid.Empty;

        public Guid SessionId =>
            Guid.TryParse(user.FindFirstValue(JwtRegisteredClaimNames.Sid), out var sessionId)
                ? sessionId
                : Guid.Empty;
    }
}
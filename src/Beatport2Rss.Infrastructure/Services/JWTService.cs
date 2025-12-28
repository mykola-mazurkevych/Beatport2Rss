using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Application.Options;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Beatport2Rss.Infrastructure.Services;

internal sealed class JwtService(
    IClock clock,
    IOptions<JwtOptions> jwtOptions) :
    IAccessTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public (AccessToken AccessToken, int ExpiresIn) Generate(User user, SessionId sessionId)
    {
        var issuedAt = clock.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.EmailAddress),
            new(JwtRegisteredClaimNames.Sid, sessionId),
            new(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            issuedAt.DateTime,
            issuedAt.AddSeconds(_jwtOptions.ExpiresIn).DateTime,
            credentials
        );
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var accessToken = AccessToken.Bearer(token);

        return (accessToken, _jwtOptions.ExpiresIn);
    }
}
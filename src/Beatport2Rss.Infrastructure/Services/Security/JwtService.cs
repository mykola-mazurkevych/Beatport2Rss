using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Infrastructure.Options;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Beatport2Rss.Infrastructure.Services.Security;

internal sealed class JwtService(
    IClock clock,
    IOptions<JwtOptions> jwtOptions) :
    IAccessTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public (AccessToken AccessToken, int ExpiresIn) Generate(UserAuthDetailsReadModel userAuthDetails, SessionId sessionId)
    {
        var issuedAt = clock.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userAuthDetails.Id),
            new(JwtRegisteredClaimNames.GivenName, userAuthDetails.FirstName ?? string.Empty),
            new(JwtRegisteredClaimNames.FamilyName, userAuthDetails.LastName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, userAuthDetails.EmailAddress),
            new(JwtRegisteredClaimNames.Sid, sessionId),
            new(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            issuedAt.UtcDateTime,
            issuedAt.AddSeconds(_jwtOptions.ExpiresIn).UtcDateTime,
            credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var accessToken = AccessToken.Bearer(token);

        return (accessToken, _jwtOptions.ExpiresIn);
    }
}
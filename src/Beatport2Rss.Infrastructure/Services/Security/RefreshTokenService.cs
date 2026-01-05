using System.Security.Cryptography;
using System.Text;

using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Application.Options;
using Beatport2Rss.Domain.Sessions;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Infrastructure.Services.Security;

internal sealed class RefreshTokenService(
    IClock clock,
    IOptions<RefreshTokenOptions> options) :
    IRefreshTokenService
{
    private readonly RefreshTokenOptions _options = options.Value;

    public (RefreshToken RefreshToken, DateTimeOffset ExpiresAt) Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var encoded = WebEncoders.Base64UrlEncode(bytes);
        var refreshToken = RefreshToken.Create(encoded);

        var expiresAt = clock.UtcNow.AddSeconds(_options.ExpiresIn);

        return (refreshToken, expiresAt);
    }

    public RefreshTokenHash Hash(RefreshToken refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = SHA256.HashData(bytes);
        var refreshTokenHash = RefreshTokenHash.Create(hash);

        return refreshTokenHash;
    }
}
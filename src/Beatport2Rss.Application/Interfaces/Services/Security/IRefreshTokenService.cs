using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Application.Interfaces.Services.Security;

public interface IRefreshTokenService
{
    (RefreshToken RefreshToken, DateTimeOffset ExpiresAt) Generate();
    RefreshTokenHash Hash(RefreshToken refreshToken);
}
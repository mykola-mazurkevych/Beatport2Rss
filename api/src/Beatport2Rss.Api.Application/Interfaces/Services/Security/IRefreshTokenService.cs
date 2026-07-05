using Beatport2Rss.Api.Domain.Sessions;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Security;

public interface IRefreshTokenService
{
    (RefreshToken RefreshToken, DateTimeOffset ExpiresAt) Generate();
    RefreshTokenHash Hash(RefreshToken refreshToken);
}
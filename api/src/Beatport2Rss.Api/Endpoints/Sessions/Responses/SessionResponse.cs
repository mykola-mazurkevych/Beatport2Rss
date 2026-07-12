using Beatport2Rss.Api.Application.Dtos.Sessions;
using Beatport2Rss.Api.Domain.Sessions;

namespace Beatport2Rss.Api.Endpoints.Sessions.Responses;

internal sealed record SessionResponse(
    string AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    string RefreshToken)
{
    public static SessionResponse Create(SessionDto dto) =>
        new(dto.AccessToken.Value,
            dto.TokenType,
            dto.ExpiresIn,
            dto.RefreshToken.Value);
}
using Beatport2Rss.Application.Dtos.Sessions;
using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.WebApi.Endpoints.Sessions.Responses;

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
            dto.RefreshToken);
}
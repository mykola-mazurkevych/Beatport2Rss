using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Application.Dtos.Sessions;

public sealed record SessionDto(
    AccessToken AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    RefreshToken RefreshToken);
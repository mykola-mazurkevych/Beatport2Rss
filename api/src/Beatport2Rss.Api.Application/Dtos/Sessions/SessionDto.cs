using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Sessions;

namespace Beatport2Rss.Api.Application.Dtos.Sessions;

public sealed record SessionDto(
    AccessToken AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    RefreshToken RefreshToken);
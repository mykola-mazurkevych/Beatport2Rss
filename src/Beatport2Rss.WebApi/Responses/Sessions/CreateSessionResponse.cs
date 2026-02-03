using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.WebApi.Responses.Sessions;

internal readonly record struct CreateSessionResponse(
    string AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    string RefreshToken)
{
    public static CreateSessionResponse Create(CreateSessionResult result) =>
        new(
            result.AccessToken,
            result.TokenType,
            result.ExpiresIn,
            result.RefreshToken);
}
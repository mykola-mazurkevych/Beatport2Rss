using Beatport2Rss.Application.UseCases.Sessions.Commands;
using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.WebApi.Responses.Sessions;

internal readonly record struct UpdateSessionResponse(
    string AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    string RefreshToken)
{
    public static UpdateSessionResponse Create(UpdateSessionResult result) =>
        new(
            result.AccessToken,
            result.TokenType,
            result.ExpiresIn,
            result.RefreshToken);
}
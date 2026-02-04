using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Infrastructure.Services.Security;

internal static class JwtEvents
{
    public static Task OnAuthenticationFailed(AuthenticationFailedContext context) =>
        Task.CompletedTask; // TODO: log context.Exception.Message

    public static async Task OnTokenValidated(TokenValidatedContext context)
    {
        var sessionQueryRepository = context.HttpContext.RequestServices.GetRequiredService<ISessionQueryRepository>();

        var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier);
        var sessionIdClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sid);

        if (userIdClaim is null ||
            sessionIdClaim is null ||
            !UserId.TryParse(userIdClaim.Value, provider: null, out var userId) ||
            !SessionId.TryParse(sessionIdClaim.Value, provider: null, out var sessionId) ||
            !(await sessionQueryRepository.ExistsAsync(userId, sessionId)))
        {
            context.Fail("Session is not valid.");
        }
    }
}
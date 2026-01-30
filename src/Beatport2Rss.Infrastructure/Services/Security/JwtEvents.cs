using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Extensions;

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

        var userId = UserId.Create(context.Principal!.Id);
        var sessionId = SessionId.Create(context.Principal!.SessionId);
        var exists = await sessionQueryRepository.ExistsAsync(userId, sessionId);
        if (!exists)
        {
            context.Fail("Session is not valid.");
        }
    }
}
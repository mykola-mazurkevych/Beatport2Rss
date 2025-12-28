using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.UseCases.Sessions.Interfaces;
using Beatport2Rss.Domain.Sessions;

using Wolverine;

namespace Beatport2Rss.Infrastructure.Middlewares;

public sealed class SessionLookupMiddleware(ISessionQueryRepository sessionQueryRepository)
{
    public async Task<(HandlerContinuation, Session?)> LoadAsync(INeedSessionRequest request, CancellationToken cancellationToken)
    {
        var sessionId = SessionId.Create(request.SessionId);
        var session = await sessionQueryRepository.GetAsync(sessionId, cancellationToken);

        return session is null
            ? (HandlerContinuation.Stop, null)
            : (HandlerContinuation.Continue, session);
    }
}
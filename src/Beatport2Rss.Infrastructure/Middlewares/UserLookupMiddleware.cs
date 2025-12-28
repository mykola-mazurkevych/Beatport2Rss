using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.UseCases.Users.Interfaces;
using Beatport2Rss.Domain.Users;

using Wolverine;

namespace Beatport2Rss.Infrastructure.Middlewares;

public sealed class UserLookupMiddleware(IUserQueryRepository userQueryRepository)
{
    public async Task<(HandlerContinuation, User?)> LoadAsync(INeedUserRequest request, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(request.UserId);
        var user = await userQueryRepository.GetAsync(userId, cancellationToken);

        return user is null
            ? (HandlerContinuation.Stop, null)
            : (HandlerContinuation.Continue, user);
    }
}
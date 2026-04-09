using Beatport2Rss.Application.Dtos.Users;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Queries;

public sealed record GetUserQuery(UserId UserId) :
    IQuery<Result<UserDto>>;

internal sealed class GetUserQueryHandler(
    IUserQueryRepository userQueryRepository) :
    IQueryHandler<GetUserQuery, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await userQueryRepository.LoadAsync(query.UserId, cancellationToken);

        return new UserDto(
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            user.IsActive,
            user.FeedsCount,
            user.TagsCount);
    }
}
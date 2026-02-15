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
        var userId = UserId.Create(query.UserId);

        var readModel = await userQueryRepository.LoadUserDetailsReadModelAsync(userId, cancellationToken);

        return new UserDto(
            readModel.EmailAddress,
            readModel.FirstName,
            readModel.LastName,
            readModel.IsActive,
            readModel.FeedsCount,
            readModel.TagsCount);
    }
}
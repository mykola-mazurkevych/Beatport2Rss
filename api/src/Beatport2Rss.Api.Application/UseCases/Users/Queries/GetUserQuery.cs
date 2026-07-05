using Beatport2Rss.Api.Application.Dtos.Users;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Users.Queries;

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
        var userDetails = await userQueryRepository.LoadUserDetailsAsync(query.UserId, cancellationToken);

        return new UserDto(
            userDetails.EmailAddress,
            userDetails.FirstName,
            userDetails.LastName,
            userDetails.IsActive,
            userDetails.FeedsCount,
            userDetails.TagsCount);
    }
}
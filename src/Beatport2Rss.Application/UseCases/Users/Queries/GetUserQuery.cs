using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Queries;

public sealed record UserDetailsResponse(
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount);

public sealed record GetUserQuery(UserId UserId) :
    IQuery<Result<UserDetailsResponse>>;

internal sealed class GetUserQueryHandler(
    IUserQueryRepository userQueryRepository) :
    IQueryHandler<GetUserQuery, Result<UserDetailsResponse>>
{
    public async ValueTask<Result<UserDetailsResponse>> Handle(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(query.UserId);

        var readModel = await userQueryRepository.LoadUserDetailsReadModelAsync(userId, cancellationToken);

        return new UserDetailsResponse(
            readModel.EmailAddress,
            readModel.FirstName,
            readModel.LastName,
            readModel.IsActive,
            readModel.FeedsCount,
            readModel.TagsCount);
    }
}
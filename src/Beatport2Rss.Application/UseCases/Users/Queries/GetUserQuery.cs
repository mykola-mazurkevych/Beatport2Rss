using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Queries;

public readonly record struct GetUserQuery(Guid UserId) :
    IQuery<Result<GetUserResult>>;

public readonly record struct GetUserResult(
    string EmailAddress,
    string? FirstName,
    string? LastName,
    int FeedsCount,
    int TagsCount);

internal sealed class GetUserQueryValidator :
    AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID required.");
    }
}

internal sealed class GetUserQueryHandler(
    IUserQueryRepository userRepository) :
    IQueryHandler<GetUserQuery, Result<GetUserResult>>
{
    public async ValueTask<Result<GetUserResult>> Handle(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(query.UserId);
        var user = await userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound("User not found.");
        }

        var result = new GetUserResult(
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            user.Feeds.Count,
            user.Tags.Count);

        return result;
    }
}
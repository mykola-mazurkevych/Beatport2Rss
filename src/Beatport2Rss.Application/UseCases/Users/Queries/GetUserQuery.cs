using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Queries;

public readonly record struct GetUserQuery(Guid UserId) :
    IQuery<Result<UserDetailsReadModel>>;

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
    IUserQueryRepository userQueryRepository) :
    IQueryHandler<GetUserQuery, Result<UserDetailsReadModel>>
{
    public async ValueTask<Result<UserDetailsReadModel>> Handle(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(query.UserId);
        var userDetails = await userQueryRepository.LoadUserDetailsReadModelAsync(userId, cancellationToken);

        return userDetails;
    }
}
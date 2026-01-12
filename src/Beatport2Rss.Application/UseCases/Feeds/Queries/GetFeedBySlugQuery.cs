using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using ErrorOr;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public readonly record struct GetFeedBySlugQuery(
    Guid UserId,
    string? Slug) :
    IQuery<ErrorOr<GetFeedBySlugResult>>, IValidate, IHaveUserId;

public readonly record struct GetFeedBySlugResult(
    string Name,
    string? Owner,
    string Slug,
    FeedStatus Status,
    DateTimeOffset CreatedAt);

public sealed class GetFeedBySlugQueryValidator :
    AbstractValidator<GetFeedBySlugQuery>
{
    public GetFeedBySlugQueryValidator()
    {
        RuleFor(q => q.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(q => q.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(Slug.MaxLength).WithMessage($"Slug must be at most {Slug.MaxLength} characters.");
    }
}

public sealed class GetFeedBySlugQueryHandler(
    IUserQueryRepository userRepository) :
    IQueryHandler<GetFeedBySlugQuery, ErrorOr<GetFeedBySlugResult>>
{
    public async ValueTask<ErrorOr<GetFeedBySlugResult>> Handle(
        GetFeedBySlugQuery query,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(query.UserId);
        var user = await userRepository.LoadAsync(userId, cancellationToken);

        var slug = Slug.Create(query.Slug);
        var feed = user.Feeds.SingleOrDefault(f => f.Slug == slug);

        if (feed is null)
        {
            return Error.NotFound(
                "Feed.NotFound",
                "Feed with the specified slug was not found.");
        }

        var result = new GetFeedBySlugResult(
            feed.Name,
            user.FullName,
            feed.Slug.Value,
            feed.Status,
            feed.CreatedDate);

        return result;
    }
}
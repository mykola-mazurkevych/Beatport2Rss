using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public readonly record struct GetFeedBySlugQuery(
    Guid UserId,
    string? Slug) :
    IQuery<Result<GetFeedBySlugResult>>, IRequireActiveUser;

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
    IQueryHandler<GetFeedBySlugQuery, Result<GetFeedBySlugResult>>
{
    public async ValueTask<Result<GetFeedBySlugResult>> Handle(
        GetFeedBySlugQuery query,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(query.UserId);
        var user = await userRepository.LoadAsync(userId, cancellationToken);

        var slug = Slug.Create(query.Slug);
        var feed = user.Feeds.SingleOrDefault(f => f.Slug == slug);

        if (feed is null)
        {
            return Result.NotFound($"Feed with the slug '{slug}' was not found.");
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
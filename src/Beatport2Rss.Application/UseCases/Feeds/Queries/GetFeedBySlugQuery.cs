using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Results;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Feeds.Queries;

public readonly record struct GetFeedBySlugQuery(
    Guid UserId,
    string? Slug);

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
    IValidator<GetFeedBySlugQuery> validator,
    IUserQueryRepository userRepository)
{
    public async Task<OneOf<Success<GetFeedBySlugResult>, ValidationFailed, InactiveUser, NotFound>> HandleAsync(
        GetFeedBySlugQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var userId = UserId.Create(query.UserId);
        var user = await userRepository.LoadAsync(userId, cancellationToken);

        if (!user.IsActive)
        {
            return new InactiveUser();
        }

        var slug = Slug.Create(query.Slug);
        var feed = user.Feeds.SingleOrDefault(f => f.Slug == slug);

        if (feed is null)
        {
            return new NotFound();
        }

        var result = new GetFeedBySlugResult(
            feed.Name,
            user.FullName,
            feed.Slug.Value,
            feed.Status,
            feed.CreatedDate);

        return new Success<GetFeedBySlugResult>(result);
    }
}
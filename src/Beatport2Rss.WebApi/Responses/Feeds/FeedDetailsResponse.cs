using Beatport2Rss.Application.ReadModels.Feeds;

namespace Beatport2Rss.WebApi.Responses.Feeds;

internal readonly record struct FeedDetailsResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Owner,
    bool IsActive,
    DateTimeOffset CreatedAt)
{
    public static FeedDetailsResponse Create(FeedDetailsReadModel readModel) =>
        new(
            readModel.Id,
            readModel.Name,
            readModel.Slug,
            readModel.Owner,
            readModel.IsActive,
            readModel.CreatedAt);
}
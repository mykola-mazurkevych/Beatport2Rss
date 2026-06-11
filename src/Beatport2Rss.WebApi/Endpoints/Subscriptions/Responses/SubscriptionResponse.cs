using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionResponse(
    int Id,
    string Name,
    string Slug,
    BeatportSubscriptionType BeatportType,
    Uri BeatportUri,
    Uri ImageUri,
    string? Country,
    IEnumerable<SubscriptionTagResponse> Tags)
{
    public static SubscriptionResponse Create(SubscriptionDto dto) =>
        new(dto.Id.Value,
            dto.Name.Value,
            dto.Slug.Value,
            dto.BeatportType,
            dto.BeatportUri,
            dto.ImageUri,
            dto.Country?.Value,
            dto.Tags.Select(SubscriptionTagResponse.Create));
}
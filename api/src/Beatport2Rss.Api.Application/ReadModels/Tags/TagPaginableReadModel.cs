using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Application.ReadModels.Tags;

public sealed class TagPaginableReadModel :
    IPaginable<TagId>
{
    public required TagId Id { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required TagName Name { get; init; }
    public required Slug Slug { get; init; }
}
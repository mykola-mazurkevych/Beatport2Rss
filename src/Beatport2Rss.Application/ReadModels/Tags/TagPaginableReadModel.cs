using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.ReadModels.Tags;

public sealed class TagPaginableReadModel :
    IPaginable<TagId>
{
    public required TagId Id { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required TagName Name { get; init; }
    public required Slug Slug { get; init; }
}
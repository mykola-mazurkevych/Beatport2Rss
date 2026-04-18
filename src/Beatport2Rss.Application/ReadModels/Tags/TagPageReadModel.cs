using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.ReadModels.Tags;

public sealed record TagPageReadModel(
    TagId Id,
    DateTimeOffset CreatedAt,
    TagName Name,
    Slug Slug) :
    IPaginable<TagId>;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Application.ReadModels.Tags;

public sealed record TagDetailsReadModel(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
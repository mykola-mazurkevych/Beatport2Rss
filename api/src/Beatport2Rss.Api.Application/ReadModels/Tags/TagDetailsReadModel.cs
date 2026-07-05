using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Tags;

namespace Beatport2Rss.Api.Application.ReadModels.Tags;

public sealed record TagDetailsReadModel(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
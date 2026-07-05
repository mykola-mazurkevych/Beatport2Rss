using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Tags;

namespace Beatport2Rss.Api.Application.Dtos.Tags;

public sealed record TagPaginableDto(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
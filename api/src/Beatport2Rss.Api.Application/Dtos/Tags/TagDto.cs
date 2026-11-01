using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.Domain.ValueObjects;

namespace Beatport2Rss.Api.Application.Dtos.Tags;

public sealed record TagDto(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
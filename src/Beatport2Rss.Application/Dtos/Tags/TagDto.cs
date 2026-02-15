using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Application.Dtos.Tags;

public sealed record TagDto(
    TagName Name,
    Slug Slug);
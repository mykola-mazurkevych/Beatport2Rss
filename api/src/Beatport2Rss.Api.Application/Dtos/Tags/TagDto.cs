using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Api.Application.Dtos.Tags;

public sealed record TagDto(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
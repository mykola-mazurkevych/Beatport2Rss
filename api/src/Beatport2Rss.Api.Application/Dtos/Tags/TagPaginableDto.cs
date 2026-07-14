using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Dtos.Tags;

public sealed record TagPaginableDto(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
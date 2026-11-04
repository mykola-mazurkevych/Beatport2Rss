using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Application.ReadModels.Tags;

public sealed record TagDetailsReadModel(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
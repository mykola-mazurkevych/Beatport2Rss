using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Api.Application.ReadModels.Tags;

public sealed record TagDetailsReadModel(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt);
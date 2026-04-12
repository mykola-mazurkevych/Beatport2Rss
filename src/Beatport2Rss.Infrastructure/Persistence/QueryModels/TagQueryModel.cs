using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Infrastructure.Persistence.QueryModels;

internal sealed record TagQueryModel(
    TagId Id,
    DateTimeOffset CreatedAt,
    UserId UserId,
    TagName Name,
    Slug Slug) :
    IQueryModel<TagId>;